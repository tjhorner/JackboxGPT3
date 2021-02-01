# JackboxGPT3

Because I wanted to use AI for party games instead of useful things.

This project is a Jackbox client controlled by GPT-3. It currently supports these games:

- Fibbage 3
- Quiplash 3

Support is planned for these games:

- Survive The Internet
- Word Spud

## Playing

You'll need to provide your OpenAI API key in the environment variable `OPENAI_KEY`.

To play a game, provide the room code for a supported game as the first parameter on the command line.

## Adding Games

If you wish to contribute and add support for a new game, it's pretty simple. Just follow this guide:

1. Create a new directory for the game inside of `src/Games`. It should be the title of the game formatted in PascalCase, e.g. `Fibbage3` or `SurviveTheInternet`.
2. In the game directory, add a new `Models` directory.
3. Create two structs to represent the game room and the game player in `Models`, named `{YourGame}Room` and `{YourGame}Player`.
4. Create a new class named `{YourGame}Client` in the game directory. It should extend `BaseJackboxClient<{YourGame}Room, {YourGame}Player>`.
5. Create a new class in `src/Engines` named `{YourGame}Engine` which extends `BaseJackboxEngine`.
6. Finally, in `Startup.cs` register your client and engine in `RegisterGameEngines` with the proper app tag.

Ok, but what do all of these things even do?

### Client

The game client handles abstracts the communication between the Jackbox server into a nice API that can be consumed by the engine.

You will need to do some reverse engineering work to understand the protocol each game uses to communicate. However, all Jackbox games share at least some similarities: the raw WebSocket data is sent in a consistent JSON structure, including a sequence identifier, opcode, and some body. The `BaseJackboxClient` will deserialize this for you and send it to your `ServerMessageReceived` method for further processing.

What you do here really depends on the game you are adding, but generally you will need to add handlers for room and player updates by overriding `ServerMessageReceived` and handling the relevant opcodes. For example, Fibbage 3 uses the opcode `text` to send objects which have a `key` and a `val` -- for room updates, the `key` is `bc:room`. When this is received, the `GameState` is updated with the room details. You can see how this is handled inside of the `ServerMessageReceived` in `Fibbage3Client`.

The API you create should reflect actions a player would take in a game. For example, the Fibbage 3 client has a method `SubmitLie` which will, you know, submit a lie that answers the prompt. You may also wish to create events in your client for room updates and player updates.

### Engine

The engine is what uses the client API you defined above and uses GPT-3 (or any other service provided via `ICompletionService`) to play the game. With `ICompletionService` you can provide the prompt, any GPT-3 parameters you want, a set of conditions for a valid response, and how many times it should try to get a valid response before giving up. You can find some good examples in the Fibbage 3 engine, under `SubmitLie`. This is called when the state changes to `EnterText` and the bot hasn't written anything yet.