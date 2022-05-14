# Application Architecture

## Prerequisites

I want to be able to provide a cross platform application, this means I want to be able to deploy to the platforms:

- Windows
- Linux
- MacOS

Mobile platforms should also be an option to be added later. That means:

- Android
- iOS

## Considerations

There are a multiple things to consider when it comes to application architecture:

- The user should be able to **install the application with relative ease**
  - This depends in part on the target audience (an it affine user will be able to understand why they need a client/server model and not just a single application)
  - If i can provide a simple and efficient setup option even a more complicated architecture is acceptable (something like a dockerized solution)
- It would be great to **decouple backend logic** (e.g. adding transacctions to the db or retrieving stock prices) from the frontend
  - A **server/client model** seems logical
  - The backend can be written in performant C# while the frontend can be kept relatively light
  - Synchronization between devices needs no additional configuration, the server only needs to be accessible to all devices
    - **Note:** Security concerns become important here. It's probably most realistic here to just recommend to users keeping the server only accessible by their own devices. Developing a secure (authenticated) service is most out of scope for now (but certainly not impossible).
  - Users could build their own applications on top of an API that I provide

## Single Application approach

Providing a single packaged application would be great in terms of deployment complexity for users. Install it like any other program, boom you are up and running. But I see multiple issues with this approach:

- Synchronization of data would need more consideration with this approach since it would need to be replicated
  - no major issue, there are solutions but could get tricky
- I would need to rewrite current C# logic in JS since I don't see another good solutions here to bridge the two worlds
  - [ElectronNet](https://github.com/ElectronNET/Electron.NET) tries to provide a solution to this problem, allowing the developer to write an Electron application combined with an ASP.NET backend
    - _Issue 1 - Added complexity:_ Building multiple layers of technolgoies onto another often comes with drawbacks and adds additional issues. It's often better to keep it as simple as possible or development can get really slow/complicated
    - _Issue 2 - Project Support:_ Electron.NET is not as big as Electron itself, has only 2 main developers which also don't work on the project as their main job. Project development and maintenance could very well cease in the future. Relying on Electron itself seems to be a safer bet.
    - _Issue 3 - Community:_ The community of Electron.NET isn't that big, issues that are particular to Electron.NET might get tricky to solve.
    - _Issue 4 - Mobile_: Electron is not built for mobile applications, if everything is baked into one application I would need to reconsider my options for a mobile release
  - [WebAssembly](https://github.com/unoplatform/Uno.Wasm.Bootstrap) is another approach which makes it possible to bring C# code to the browser
    - _Issue 1 - Experimental technology:_ Packaging C# code into a .wasm file is not direcctly supported by Microsoft (they only support this via their [Blazor](https://dotnet.microsoft.com/en-us/apps/aspnet/web-apps/blazor) framework - which itself could very well be another doomed project due to relatively low adoption)
    - _Issue 2 - Added complexity:_ Using WebAssembly is still very new and not well established. Problems could slow development down a lot and seem quite likely since running C# code in WebAssembly is extremly experimental outside of Blazor. It doesn't seem worth it.

## Decission

Using a **client/server model** seems to be the most realistic option. Drawdowns of the single application approach are too great. Deployment of the application will be a little harder for the user but I should be able to make it manageable (by providing a good deployment option).

### Architecture Sketch

The application architecture should look roughly like this:
