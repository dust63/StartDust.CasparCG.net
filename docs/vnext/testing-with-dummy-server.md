# Testing with DummyServer

`DummyServer` support lives in `StarDust.CasparCG.Testing` and provides a lightweight loopback harness for integration tests.

## Example

```csharp
await using var server = await DummyCasparServer.StartAsync(
    DummyScenario.Empty().WithAmcpReply("PLAY 1-10 AMB", "202 PLAY OK\r\n"),
    CancellationToken.None);

var client = new CasparClient(new TcpAmcpTransport("127.0.0.1", server.AmcpPort));
await client.ConnectAsync(CancellationToken.None);
await client.PlayAsync(1, 10, "AMB", CancellationToken.None);
```

## Expected usage

- script the AMCP replies needed by the test
- point `TcpAmcpTransport` at `server.AmcpPort`
- assert against `server.ReceivedCommands`
