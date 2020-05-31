using System.Threading.Tasks;
using FluentAssertions;
using SharpSettingsStore.Impl;
using Xunit;

namespace SharpSettingsStore.Tests
{
    public class TypedSettingsTests : BaseTests
    {
        [SettingName("test-setting")]
        public class RegionalSettings
        {
            public RegionalSettings()
            {
                Culture = "en-US";
                Language = "English";
            }
            
            public string Culture { get; set; }
            
            public string Language { get; set; }
        }
        private readonly ITypedSettingsService<RegionalSettings> _regionalService;

        public TypedSettingsTests()
        {
            _regionalService = new TypedSettingsService<RegionalSettings>(new SettingsService(GetDataService(), new SettingsSerializer()), null);
        }

        [Fact]
        public async Task Settings_should_have_defaults()
        {
            var setting = await _regionalService.GetSettings();
            setting.Culture.Should().Be("en-US");
            setting.Language.Should().Be("English");
        }
        
        [Fact]
        public async Task Can_save_settings()
        {
            await _regionalService.SaveSettings(settings =>
            {
                settings.Culture = "es";
                settings.Language = "Espanol";
            });
            
            var updatedSetting = await _regionalService.GetSettings();
            updatedSetting.Culture.Should().Be("es");
            updatedSetting.Language.Should().Be("Espanol");
        }
    }
}