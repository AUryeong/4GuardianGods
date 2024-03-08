using System;

public class Singleton<T> where T : class
{
    private static T instance;
    
    public static T Instance => instance ??= Activator.CreateInstance<T>();
}