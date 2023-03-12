# kube.net

## What?
Manage your k8s configuration file with this simple CLI tool.

## Why?
I wanted something simple to manage my k8s config. Because I mostly use [k9s](https://k9scli.io/) and other CLI tools I wanted that thing to also be available on the CLI.

Oh, why C#? Well, I wanted to do something in C# because I didn't have any experience with it. (And Spectre Console looked awesome).

## Features
- Remove a context
- Switch active context
- Import a new context from clipboard

## How to run
Run `dotnet publish -c Release --sc`. This should result in a single binary (`kube.net`).
The binary is published to `bin/Release/net7.0/osx-arm64/publish/`, so if the copy fails you can copy it yourself.

To run it, simply execute the binary. To exit, press Ctrl+C.

## Importing a context
The option 'import from clipboard' fetches the contents of your clipboard and tries to parse it as a kubeconfig. If successful, the new context should be visible in the list.

## Todo
- UX: confirmations here and there, maybe some info on the selected context (and the about-to-be-imported context)
- Expand the KubeConfigDef class to avoid losing data on import+write.