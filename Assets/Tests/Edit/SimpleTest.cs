using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SimpleTest
{

    [Test]
    public void SimpleTestSimplePasses()
    {
        Assert.AreEqual(1, 1);
    }
}
