namespace Bot.Requests.Item;

public record GetItemMessageText(Core.Models.Item Item) : IRequest<string>;