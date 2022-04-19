using System;

namespace XMapper.Testing.Tests;

public static class Does
{
    public static void NotThrow(Action action)
    {
        action();
    }
}
