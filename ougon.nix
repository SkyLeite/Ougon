{ fetchFromGitHub, buildDotnetModule, lib, dotnet-sdk_7 }:

buildDotnetModule rec {
  pname = "ougon";
  version = "0.0.1";

  src = ./.;

  nugetDeps = ./deps.nix;

  dotnet-sdk = dotnet-sdk_7;

  projectFile = "Ougon.sln";

  meta = with lib; {
    homepage = "some_homepage";
    description = "some_description";
    license = licenses.mit;
  };
}
