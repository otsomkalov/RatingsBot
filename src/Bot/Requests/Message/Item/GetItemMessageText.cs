namespace Bot.Requests.Message.Item;

public record GetItemMessageText(Core.Models.Item Item) : IRequest<string>;