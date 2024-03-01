﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (https://www.specflow.org/).
//      SpecFlow Version:3.9.0.0
//      SpecFlow Generator Version:3.9.0.0
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace MediaManager.ArchivingRulesTests.Features
{
    using TechTalk.SpecFlow;
    using System;
    using System.Linq;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [Xunit.TraitAttribute("Category", "MediaTypeArchivingRule")]
    public partial class MediaTypeArchivingRuleFeature : object, Xunit.IClassFixture<MediaTypeArchivingRuleFeature.FixtureData>, System.IDisposable
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        private string[] _featureTags = new string[] {
                "MediaTypeArchivingRule"};
        
        private Xunit.Abstractions.ITestOutputHelper _testOutputHelper;
        
#line 1 "MediaTypeArchivingRule.feature"
#line hidden
        
        public MediaTypeArchivingRuleFeature(MediaTypeArchivingRuleFeature.FixtureData fixtureData, MediaManager_ArchivingRulesTests_XUnitAssemblyFixture assemblyFixture, Xunit.Abstractions.ITestOutputHelper testOutputHelper)
        {
            this._testOutputHelper = testOutputHelper;
            this.TestInitialize();
        }
        
        public static void FeatureSetup()
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Features", "MediaTypeArchivingRule", "The MediaTypeArchivingRule feature focuses on managing archiving calls based on m" +
                    "edia type,\r\ncovering scenarios for applying the media type archiving rule\r\nand r" +
                    "efreshing media type when configuration changes.", ProgrammingLanguage.CSharp, new string[] {
                        "MediaTypeArchivingRule"});
            testRunner.OnFeatureStart(featureInfo);
        }
        
        public static void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        public virtual void TestInitialize()
        {
        }
        
        public virtual void TestTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioInitialize(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioInitialize(scenarioInfo);
            testRunner.ScenarioContext.ScenarioContainer.RegisterInstanceAs<Xunit.Abstractions.ITestOutputHelper>(_testOutputHelper);
        }
        
        public virtual void ScenarioStart()
        {
            testRunner.OnScenarioStart();
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        public virtual void FeatureBackground()
        {
#line 8
#line hidden
#line 9
 testRunner.Given("the media type archiving rule is initialized", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
#line hidden
#line 10
 testRunner.And("an initial media type configuration", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
        }
        
        void System.IDisposable.Dispose()
        {
            this.TestTearDown();
        }
        
        [Xunit.SkippableTheoryAttribute(DisplayName="Apply media type archiving rule with correct media types")]
        [Xunit.TraitAttribute("FeatureTitle", "MediaTypeArchivingRule")]
        [Xunit.TraitAttribute("Description", "Apply media type archiving rule with correct media types")]
        [Xunit.TraitAttribute("Category", "ApplyMediaTypeArchivingRule")]
        [Xunit.InlineDataAttribute("1", "Voice,Voice,Screen", "1,2", new string[0])]
        [Xunit.InlineDataAttribute("2", "Screen", "", new string[0])]
        public virtual void ApplyMediaTypeArchivingRuleWithCorrectMediaTypes(string callId, string mediaType, string expectedResult, string[] exampleTags)
        {
            string[] @__tags = new string[] {
                    "ApplyMediaTypeArchivingRule"};
            if ((exampleTags != null))
            {
                @__tags = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Concat(@__tags, exampleTags));
            }
            string[] tagsOfScenario = @__tags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("CallId", callId);
            argumentsOfScenario.Add("MediaType", mediaType);
            argumentsOfScenario.Add("ExpectedResult", expectedResult);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Apply media type archiving rule with correct media types", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 13
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 8
this.FeatureBackground();
#line hidden
#line 14
 testRunner.And(string.Format("a call event with special CallId {0} and MediaType {1}", callId, mediaType), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
#line hidden
#line 15
 testRunner.When("the media type archiving rule is applied", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 16
 testRunner.Then(string.Format("it should return expected result {0} set of recording ids", expectedResult), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [Xunit.SkippableTheoryAttribute(DisplayName="Refresh media types when configuration changes")]
        [Xunit.TraitAttribute("FeatureTitle", "MediaTypeArchivingRule")]
        [Xunit.TraitAttribute("Description", "Refresh media types when configuration changes")]
        [Xunit.TraitAttribute("Category", "RefreshMediaType")]
        [Xunit.InlineDataAttribute("Voice", "MediaTypeRulConfig changed. Refreshed MediaTypes", new string[0])]
        [Xunit.InlineDataAttribute("Screen", "MediaTypeRulConfig changed. Refreshed MediaTypes", new string[0])]
        public virtual void RefreshMediaTypesWhenConfigurationChanges(string updatedMediaType, string expectedLogMessage, string[] exampleTags)
        {
            string[] @__tags = new string[] {
                    "RefreshMediaType"};
            if ((exampleTags != null))
            {
                @__tags = System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Concat(@__tags, exampleTags));
            }
            string[] tagsOfScenario = @__tags;
            System.Collections.Specialized.OrderedDictionary argumentsOfScenario = new System.Collections.Specialized.OrderedDictionary();
            argumentsOfScenario.Add("UpdatedMediaType", updatedMediaType);
            argumentsOfScenario.Add("ExpectedLogMessage", expectedLogMessage);
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Refresh media types when configuration changes", null, tagsOfScenario, argumentsOfScenario, this._featureTags);
#line 24
this.ScenarioInitialize(scenarioInfo);
#line hidden
            bool isScenarioIgnored = default(bool);
            bool isFeatureIgnored = default(bool);
            if ((tagsOfScenario != null))
            {
                isScenarioIgnored = tagsOfScenario.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((this._featureTags != null))
            {
                isFeatureIgnored = this._featureTags.Where(__entry => __entry != null).Where(__entry => String.Equals(__entry, "ignore", StringComparison.CurrentCultureIgnoreCase)).Any();
            }
            if ((isScenarioIgnored || isFeatureIgnored))
            {
                testRunner.SkipScenario();
            }
            else
            {
                this.ScenarioStart();
#line 8
this.FeatureBackground();
#line hidden
#line 25
 testRunner.When(string.Format("the configuration changes to updated media type {0}", updatedMediaType), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
#line hidden
#line 26
 testRunner.Then(string.Format("it should refresh media types and log the change with message {0}", expectedLogMessage), ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
#line hidden
            }
            this.ScenarioCleanup();
        }
        
        [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "3.9.0.0")]
        [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
        public class FixtureData : System.IDisposable
        {
            
            public FixtureData()
            {
                MediaTypeArchivingRuleFeature.FeatureSetup();
            }
            
            void System.IDisposable.Dispose()
            {
                MediaTypeArchivingRuleFeature.FeatureTearDown();
            }
        }
    }
}
#pragma warning restore
#endregion
