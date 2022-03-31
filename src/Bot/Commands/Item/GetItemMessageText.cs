namespace Bot.Commands.Item;

public record GetItemMessageText(Core.Models.Item Item) : IRequest<string>;