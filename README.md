# Rocco.RelayServer

[![CI](https://github.com/RoccoDevs/RoccoRelayServer/actions/workflows/dotnet-core.yml/badge.svg)](https://github.com/RoccoDevs/RoccoRelayServer/actions/workflows/dotnet-core.yml)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/e2bce1c016a04b50ae2e5650c10e42d2)](https://app.codacy.com/gh/RoccoDevs/RoccoRelayServer/dashboard?utm_source=gh&utm_medium=referral&utm_content=&utm_campaign=Badge_grade)
[![Codacy Badge](https://app.codacy.com/project/badge/Coverage/e2bce1c016a04b50ae2e5650c10e42d2)](https://app.codacy.com/gh/RoccoDevs/RoccoRelayServer/dashboard?utm_source=gh&utm_medium=referral&utm_content=&utm_campaign=Badge_coverage)
[![Codacy Security Scan](https://github.com/RoccoDevs/RoccoRelayServer/actions/workflows/codacy-analysis.yaml/badge.svg)](https://github.com/RoccoDevs/RoccoRelayServer/actions/workflows/codacy-analysis.yaml)

## Introduction

RoccoRelayServer is a fast server meant to facilitate P2P communication between clients over the internet. The main goal is to bypass anything like UPnP or port forwarding.

## History

I created this for a university project at the HAN that specified our P2P game could not use features like UPnP or NAT hole punching to connect clients. That meant we needed something like a VPN. Instead, I came up with this relay server as it seemed like a good alternative. And perhaps most importantly, I saw it as a good opportunity to learn new things.

## SixtyNine protocol

The protocol is fast and barebones. It only contains the length and payload. It uses four types to determine what the message is and how the relay should handle it. `INIT,CLOSE,ERROR,MESSAGE`

### Protocol overview

|             | Length                                                                     | Payload                                                          |
|-------------|----------------------------------------------------------------------------|------------------------------------------------------------------|
| Encoding    | Big-Endian                                                                 | UTF8                                                             |
| Type        | INT32                                                                      | string                                                           |
| Max length  | 2147483647                                                                 | Depends on platform                                              |
| Formatting  | Plain text                                                                 | JSON                                                             |
| Description | This is the length of the payload. That is necessary for the decoding system. | The payload to be sent. More details in "Payload examples" |

### Payload

|       | payloadType               | destination            | source            | payloadContent |
|-------|---------------------------|------------------------|-------------------|----------------|
| Type  | string                    | string                 | string            | string         |
| Value | PayloadTypeEnum as string | Id of destination peer | Id of source peer | Content        |

## Payload examples

As stated earlier, the protocol has three message types: `INIT, CLOSE, ERROR, MESSAGE.` The relay server uses these types to determine what to do.

### INIT - initialize connection

To initialize a connection between a peer and the relay server *for the first time*, the client must send an `INIT` message. This message should look like this:

#### Client-side init

```text
LENGTH OF PAYLOAD
{
        INIT,  
        null,
        null,
        null
}
```

If the client wishes to reconnect to the server, add a `sourceId` to the message:

```text
LENGTH OF PAYLOAD
{
        INIT,  
        sourceId,
        null,
        null
}
```

#### Server-side init

If successful, the server will reply like this:

```text
LENGTH OF PAYLOAD
{
    INIT,  
    null,
    sourceId,
    null
}
```

### Close - terminate the connection

If a client needs to be disconnected, it can do so with a `CLOSE` message:

#### Client-side close

```text
LENGTH OF PAYLOAD
{
        CLOSE,  
        null,
        null,
        null
}
```

### ERROR - handling errors

When the server errors while handling a message, it'll try to return an error to the `source client.` Clients can also use error messages to notify a source that something went wrong when receiving their message.

#### Client-side error

```text
LENGTH OF PAYLOAD
{
    ERROR,  
    sourceId,
    destinationId,
    payloadContent
}
```

#### Server side error

When an error gets thrown by the server, it'll try to return a message to the original `sender`:

```text
LENGTH OF PAYLOAD
{
    ERROR,  
    null,
    destinationId,
    payloadContent
}
```

### Message - sending messages

To send messages between connected clients, use the `MESSAGE` payload type.

```text
LENGTH OF PAYLOAD
{
    MESSAGE,  
    sourceId,
    destinationId,
    payloadContent
}
```

## How do clients know about each other?

They don't. In our project, we had a `bootstrap` client. It consisted of a permanently connected client. It contained a list of all connected clients, much like the relay. When a new client wanted to connect, all they needed to do was connect to that `bootstrap` client. However, the code for that client was not entirely written by me, and I do not have the right to publish it.

## Client library?

The client library for this was written in Java. Sadly, I cannot publish it because I wasn't the only contributor to that part of the project. However you can easily make one with the `reader` and `writer` supplied in `Rocco.RelayServer.Core`.

## Getting Started

### Prerequisites

You'll need the following tools:

* [Git](https://git-scm.com/)
* [Dotnet 7](https://dotnet.microsoft.com/download/dotnet/7.0)
* Your favorite dotnet capable IDE or text editor

## Build and Test

### Build

#### Dotnet

After cloning sources, navigate into the newly cloned folder and build using `dotnet`:

```text
dotnet build
```

Alternatively, open the solution with your favorite dotnet capable IDE.

Visual studio works too by double-clicking on the .sln file.

### Run

Select pre-configured launch configs from your IDE

### Debug

Select pre-configured launch configs from your IDE
