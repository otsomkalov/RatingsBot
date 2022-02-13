namespace Bot.Commands.Item;

public record GetItemMessageText(Models.Item Item) : IRequest<string>;