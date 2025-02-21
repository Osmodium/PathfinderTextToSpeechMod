using Moq;
using SpeechMod;
using SpeechMod.Voice;
using Xunit;

namespace Test;

public class StringManipulationTests
{
    public StringManipulationTests()
    {
        Main.Settings = new()
        {
            NarratorPitch = 0,
            NarratorVoice = 0,
            NarratorRate = 0,
            NarratorVolume = 100,
            FemalePitch = 0,
            FemaleVoice = 1,
            FemaleRate = 0,
            FemaleVolume = 100,
            MalePitch = 0,
            MaleVoice = 2,
            MaleRate = 0,
            MaleVolume = 100
        };
        Main.Settings.AvailableVoices = new[] { "Narrator", "Female", "Male" };
    }

    [Theory]
    [MemberData(nameof(GenerateMaleDialogTexts))]
    public void PrepareMaleDialogText(string input, string output)
    {
        // Arrange
        Mock<WindowsSpeech> mock = new();
        mock.SetupGet(w => w.CombinedDialogVoiceStart).Returns(mock.Object.CombinedMaleVoiceStart);

        // Act
        var text = mock.Object.PrepareDialogText(input);

        // Assert
        Assert.Equal(output, text);
    }

    [Theory]
    [MemberData(nameof(GenerateFemaleDialogTexts))]
    public void PrepareFemaleDialogText(string input, string output)
    {
        // Arrange
        Mock<WindowsSpeech> mock = new();
        mock.SetupGet(w => w.CombinedDialogVoiceStart).Returns(mock.Object.CombinedFemaleVoiceStart);

        // Act
        var text = mock.Object.PrepareDialogText(input);

        // Assert
        Assert.Equal(output, text);
    }

    public static TheoryData<string, string> GenerateMaleDialogTexts()
    {
        return new TheoryData<string, string>
        {
            {
                "Here we are again, Commander. <color=#616060>Liotr looks grim but focused.</color> So let us take another glimpse into the past",
                "<voice required=\"Name=Male\"><pitch absmiddle=\"0\"/><rate absspeed=\"0\"/><volume level=\"100\"/>Here we are again, Commander. </voice><voice required=\"Name=Narrator\"><pitch absmiddle=\"0\"/><rate absspeed=\"0\"/><volume level=\"100\"/>Liotr looks grim but focused.</voice><voice required=\"Name=Male\"><pitch absmiddle=\"0\"/><rate absspeed=\"0\"/><volume level=\"100\"/> So let us take another glimpse into the past</voice>"
            },
            {
                "<color=#616060>The booming voice of an old man dressed in I-o-mædæan robes shakes the walls of the hall.</color> Get away from him, demon! Let the boy go. By the blade of the Inheritor, you touch him only over my dead body",
                "<voice required=\"Name=Narrator\"><pitch absmiddle=\"0\"/><rate absspeed=\"0\"/><volume level=\"100\"/>The booming voice of an old man dressed in I-o-mædæan robes shakes the walls of the hall.</voice><voice required=\"Name=Male\"><pitch absmiddle=\"0\"/><rate absspeed=\"0\"/><volume level=\"100\"/> Get away from him, demon! Let the boy go. By the blade of the Inheritor, you touch him only over my dead body</voice>"
            }
        };
    }

    public static TheoryData<string, string> GenerateFemaleDialogTexts()
    {
        return new TheoryData<string, string>
        {
            {
                "Here we are again, Commander. <color=#616060>Liotr looks grim but focused.</color> So let us take another glimpse into the past",
                "<voice required=\"Name=Female\"><pitch absmiddle=\"0\"/><rate absspeed=\"0\"/><volume level=\"100\"/>Here we are again, Commander. </voice><voice required=\"Name=Narrator\"><pitch absmiddle=\"0\"/><rate absspeed=\"0\"/><volume level=\"100\"/>Liotr looks grim but focused.</voice><voice required=\"Name=Female\"><pitch absmiddle=\"0\"/><rate absspeed=\"0\"/><volume level=\"100\"/> So let us take another glimpse into the past</voice>"
            },
            {
                "<color=#616060>The booming voice of an old man dressed in I-o-mædæan robes shakes the walls of the hall.</color> Get away from him, demon! Let the boy go. By the blade of the Inheritor, you touch him only over my dead body",
                "<voice required=\"Name=Narrator\"><pitch absmiddle=\"0\"/><rate absspeed=\"0\"/><volume level=\"100\"/>The booming voice of an old man dressed in I-o-mædæan robes shakes the walls of the hall.</voice><voice required=\"Name=Female\"><pitch absmiddle=\"0\"/><rate absspeed=\"0\"/><volume level=\"100\"/> Get away from him, demon! Let the boy go. By the blade of the Inheritor, you touch him only over my dead body</voice>"
            }
        };
    }
}