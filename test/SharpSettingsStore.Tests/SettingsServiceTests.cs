using System;
using System.Threading.Tasks;
using FluentAssertions;
using ServiceStack.OrmLite;
using SharpDataAccess.Data;
using SharpSettingsStore.Impl;
using Xunit;

namespace SharpSettingsStore.Tests
{
    public class SettingsServiceTests : BaseTests
    {
        private readonly ISettingsService _settingsService;
        
        public SettingsServiceTests()
        {
            _settingsService = new SettingsService(GetDataService(), new SettingsSerializer());
        }

        [SettingName("dbname")]
        private class TestSetting
        {
            public TestSetting()
            {
                NameProperty = "defaultname";
                Value = 0;
            }
            public string NameProperty { get; set; }
            public int Value { get; set; }
        }

        private class InvalidTestSettingNoAttribute
        {
            public string NameProperty { get; set; }
            public int Value { get; set; }
        }
        
        [SettingName(null)]
        private class InvalidTestSettingAttributeNoName
        {
            public string NameProperty { get; set; }
            public int Value { get; set; }
        }
        
        [Fact]
        public async Task Can_save_settings()
        {
            // if the setting doesn't exist, should provide a default setting
            var setting = await _settingsService.GetSetting<TestSetting>();
            setting.Should().NotBe(null);
            setting.NameProperty.Should().Be("defaultname");
            setting.Value.Should().Be(0);
            
            setting = new TestSetting
            {
                NameProperty = "lando",
                Value = 5
            };
            await _settingsService.SaveSetting(setting);

            setting = await _settingsService.GetSetting<TestSetting>();

            setting.NameProperty.Should().Be("lando");
            setting.Value.Should().Be(5);
        }

        [Fact]
        public async Task Can_update_settings()
        {
            var setting = new TestSetting
            {
                NameProperty = "lando",
                Value = 5
            };
            await _settingsService.SaveSetting(setting);
            
            setting = new TestSetting
            {
                NameProperty = "landocalrissian",
                Value = 10
            };
            await _settingsService.SaveSetting(setting);

            using (var conScope = new ConScope(GetDataService()))
            {
                (await conScope.Connection.CountAsync(conScope.Connection.From<SettingV1>().Where(x => x.Name == "dbname"))).Should().Be(1);
            }
            
            setting = await _settingsService.GetSetting<TestSetting>();
            setting.NameProperty.Should().Be("landocalrissian");
        }

        [Fact]
        public async Task Throws_if_setting_name_not_set()
        {
            await Assert.ThrowsAsync<Exception>(() => _settingsService.SaveSetting(new InvalidTestSettingNoAttribute()));
            await Assert.ThrowsAsync<Exception>(() => _settingsService.SaveSetting(new InvalidTestSettingAttributeNoName()));
        }
    }
}