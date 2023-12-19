using System.Reflection;
using System.Runtime.InteropServices;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using Ougon.Data;

var cheatTablePath = args.ElementAtOrDefault(0);
if (cheatTablePath == null)
{
    throw new ArgumentNullException(cheatTablePath);
}

var ct = XDocument.Load(cheatTablePath);

var ougonStructures = new List<Type>
{
    typeof(Match),
    typeof(GameCharacter),
    typeof(Frame),
    typeof(Sequence),
    typeof(SpriteInfo),
    typeof(SequenceHeader),
    typeof(LZLRFile),
    typeof(GameCharacterInMatch)
};

foreach (var structure in ougonStructures)
{
    var existingStructures = ct.XPathSelectElements(
        $"/CheatTable/Structures/Structure[@Name='{structure.Name}']"
    );

    foreach (var existingStructure in existingStructures)
    {
        existingStructure.Remove();
    }

    var structureArray = ct.XPathSelectElement($"/CheatTable/Structures");
    if (structureArray == null)
        continue;

    var newStructureElementElements = new XElement("Elements");

    foreach (var field in structure.GetFields())
    {
        var offset = field.GetCustomAttribute<FieldOffsetAttribute>();
        if (offset == null)
            continue;

        var existingOffset = newStructureElementElements.XPathSelectElement(
            $"/Element[@Offset='{offset.Value}']"
        );

        if (existingOffset != null)
        {
            continue;
        }

        var existingDescription = newStructureElementElements.XPathSelectElement(
            $"/Element[@Description='{field.Name}']"
        );

        if (existingDescription != null)
        {
            continue;
        }

        var element = new XElement(
            "Element",
            new XAttribute("Offset", offset.Value),
            new XAttribute("Vartype", "2 Bytes"),
            new XAttribute("Bytesize", "2"),
            new XAttribute("OffsetHex", offset.Value.ToString("X8")),
            new XAttribute("Description", field.Name),
            new XAttribute("DisplayMethod", "unsigned integer")
        );

        newStructureElementElements.Add(element);
    }

    if (newStructureElementElements.Descendants().Any())
        continue;

    var newStructureElement = new XElement(
        "Structure",
        new XAttribute("Name", structure.Name),
        new XAttribute("AutoFill", "0"),
        new XAttribute("AutoCreate", "1"),
        new XAttribute("DefaultHex", "0"),
        new XAttribute("AutoDestroy", "0"),
        new XAttribute("DoNotSaveLocal", "0"),
        new XAttribute("RLECompression", "1"),
        new XAttribute("AutoCreateStructsize", "4096"),
        newStructureElementElements
    );

    structureArray.Add(newStructureElement);
}

ct.Add("\n");

var settings = new XmlWriterSettings();
settings.Indent = true;
settings.IndentChars = "  ";
settings.Encoding = System.Text.Encoding.ASCII;
settings.NewLineChars = "\r\n";

var destinationFile = cheatTablePath + ".new.CT";
using (var writer = XmlWriter.Create(destinationFile, settings))
{
    ct.WriteTo(writer);
    writer.Flush();
}
;

// Cheat tables must be encoded as ASCII, but the encoding must be declared as UTF-8
// No, this is not a joke
// Yes, I lost many hours to this
var ruinedFile = File.ReadAllText(destinationFile)
    .Replace("us-ascii", "utf-8", System.StringComparison.CurrentCultureIgnoreCase);
File.WriteAllText(destinationFile, ruinedFile);

return 0;
