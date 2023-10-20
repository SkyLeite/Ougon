{
  description = "virtual environments";

  inputs.devshell.url = "github:numtide/devshell";
  inputs.flake-utils.url = "github:numtide/flake-utils";

  inputs.flake-compat = {
    url = "github:edolstra/flake-compat";
    flake = false;
  };

  outputs = { self, flake-utils, devshell, nixpkgs, ... }:
    flake-utils.lib.eachDefaultSystem (system:
      let
        pkgs = import nixpkgs {
          inherit system;

          overlays = [ devshell.overlays.default ];
        };
      in {
        packages.default = pkgs.callPackage ./ougon.nix { };

        devShell = pkgs.devshell.mkShell {
          imports = [ (pkgs.devshell.importTOML ./devshell.toml) ];

          env = [{
            name = "DOTNET_ROOT";
            value =
              "/nix/store/f0xrdhmving7afl5kz16bc4xyzj56b45-dotnet-sdk-7.0.306";
          }];
        };

      });
}
