# Ratings Telegram Bot

Telegram Bot to store and create any items.

## Getting Started

### Prerequisites

- [.NET 6](https://dotnet.microsoft.com/download) or higher
- [PostgreSQL](https://www.postgresql.org/)

### Installing

**Telegram:**

1. Contact to [@BotFather](https://t.me/BotFather) in Telegram
2. Create new bot
3. Copy bot token

**Project:**

1. Clone project
2. Update **appsettings.json**
3. Run in folder:

```
docker-compose run -d ratingsbot <telegram_bot_token>
```

or go to **Bot** folder and run:

```
dotnet run <telegram_bot_token>
```

or use existing image from Docker Hub and pass settings as environment variables:

```
docker run infinitu1327/ratingsbot
```

## Usage

You can try this bot in [Telegram](https://t.me/rtngsbot)

## Built With

* [Telegram.Bot](https://github.com/TelegramBots/Telegram.Bot) - .NET Client for Telegram Bot API
* [MediatR](https://github.com/jbogard/MediatR) - Simple, unambitious mediator implementation in .NET

## Contributing

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.