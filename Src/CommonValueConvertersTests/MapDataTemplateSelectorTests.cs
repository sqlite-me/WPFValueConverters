using Microsoft.VisualStudio.TestTools.UnitTesting;
using CommonValueConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using CommonValueConverters.Converters;

namespace CommonValueConverters.Tests
{
    [TestClass()]
    public class MapDataTemplateSelectorTests
    {
        [TestMethod()]
        public void SelectTemplateTest()
        {
            var def = new DataTemplate();
            MapDataTemplateSelector mapDataTemplateSelector = new MapDataTemplateSelector() {
            PropertyPath= "Data.Data.Name"//,Default= def
            };
            mapDataTemplateSelector.Mappings.Add(new Mapping<DataTemplate>("MyName1",new DataTemplate()));
            mapDataTemplateSelector.Mappings.Add(new Mapping<DataTemplate>("MyName", def));
           var rlt = mapDataTemplateSelector.SelectTemplate(new TestData { Data=new () {Data=new() { Name = "MyName" } } }, null);

            Assert.AreSame(mapDataTemplateSelector.Mappings.First(t=>t.From== "MyName").To,rlt);
        }

        [TestMethod()]
        public void SelectTemplateTest2()
        {
            var def = new DataTemplate();
            MapDataTemplateSelector mapDataTemplateSelector = new MapDataTemplateSelector()
            {
                PropertyPath = "."//,Default= def
            };
            mapDataTemplateSelector.Mappings.Add(new Mapping<DataTemplate>("MyName1", new DataTemplate()));
            mapDataTemplateSelector.Mappings.Add(new Mapping<DataTemplate>("MyName", def));
            var rlt = mapDataTemplateSelector.SelectTemplate(new TestData { Data = new() { Data = new() { Name = "MyName" } } }, null);

            Assert.AreSame(null, rlt);
        }

        class TestData
        {
            public TestData Data { get; set; }
            public string Name { get; set; }
        }
    }
}