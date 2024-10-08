﻿/*----------------------------------------------------------------
 *  Copyright (c) ThoughtWorks, Inc.
 *  Licensed under the Apache License, Version 2.0
 *  See LICENSE.txt in the project root for license information.
 *----------------------------------------------------------------*/

namespace Gauge.CSharp.Lib.UnitTests;

[TestFixture]
public class DefaultClassInstanceManagerTests
{
    [SetUp]
    public void SetUp()
    {
        DataStoreFactory.ClearAllDataStores();
    }

    [Test]
    public void ShouldGetInstanceForType()
    {
        var type = typeof(object);
        var instance = new DefaultClassInstanceManager().Get(type);

        Assert.NotNull(instance);
        Assert.AreEqual(instance.GetType(), type);
    }

    [Test]
    public void ShouldGetMemoizedInstanceForType()
    {
        var instanceManager = new DefaultClassInstanceManager();
        var type = typeof(object);
        var instance = instanceManager.Get(type);
        var anotherInstance = instanceManager.Get(type);

        Assert.AreSame(instance, anotherInstance);
    }

    [Test]
    public async Task InvokeMethod_ShouldCreateInstanceAndInvokeMethod_WhenInstanceIsNotCached()
    {
        var instanceManager = new DefaultClassInstanceManager();
        var methodInfo = typeof(MethodInvokeTests).GetMethod(nameof(MethodInvokeTests.InvokeMethod1));
        MethodInvokeTests.InvokeMethod1Called = false;

        await instanceManager.InvokeMethod(methodInfo, 1);

        Assert.IsTrue(MethodInvokeTests.InvokeMethod1Called);
    }

    [Test]
    public async Task InvokeMethod_ShouldSetDataStores_WhenDataStoresAreInitialized()
    {
        var instanceManager = new DefaultClassInstanceManager();
        DataStoreFactory.AddDataStore(1, DataStoreType.Suite);
        DataStoreFactory.AddDataStore(1, DataStoreType.Spec);
        DataStoreFactory.AddDataStore(1, DataStoreType.Scenario);
        var methodInfo = typeof(MethodInvokeTests).GetMethod(nameof(MethodInvokeTests.InvokeMethod2));
        MethodInvokeTests.InvokeMethod2Called = false;

        await instanceManager.InvokeMethod(methodInfo, 1);

        Assert.IsTrue(MethodInvokeTests.InvokeMethod2Called);
    }

    [Test]
    public async Task InvokeMethod_ShouldNotFail_WhenMethodInvokedIsNotAsync()
    {
        var instanceManager = new DefaultClassInstanceManager();
        var methodInfo = typeof(MethodInvokeTests).GetMethod(nameof(MethodInvokeTests.InvokeMethod3));
        MethodInvokeTests.InvokeMethod3Called = false;

        await instanceManager.InvokeMethod(methodInfo, 1);

        Assert.IsTrue(MethodInvokeTests.InvokeMethod3Called);
    }
}

public class MethodInvokeTests
{
    public static bool InvokeMethod1Called { get; set; }
    public static bool InvokeMethod2Called { get; set; }
    public static bool InvokeMethod3Called { get; set; }

    public Task InvokeMethod1()
    {
        InvokeMethod1Called = true;
        return Task.CompletedTask;
    }

    public Task InvokeMethod2()
    {
        InvokeMethod2Called = true;
        Assert.IsNotNull(SuiteDataStore.Store.Value);
        Assert.IsNotNull(SpecDataStore.Store.Value);
        Assert.IsNotNull(ScenarioDataStore.Store.Value);
        return Task.CompletedTask;
    }

    public void InvokeMethod3()
    {
        InvokeMethod3Called = true;
    }
}