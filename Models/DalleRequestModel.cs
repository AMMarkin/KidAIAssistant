namespace TheOneWhoWritesYourSpeech.Models;

/// <summary>
/// Модель запроса к Dall-E
/// </summary>
public class DalleRequestModel
{
    /// <summary>
    /// Id модели
    /// </summary>
    public string Model { get; set; } = "dall-e-2";

    /// <summary>
    /// Описание картинки
    /// </summary>
    public required string Prompt { get; set; }

    /// <summary>
    /// Размер картинки
    /// </summary>
    public string Size { get; set; } = "256x256";


}
