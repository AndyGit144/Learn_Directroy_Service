using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Domain.Enums
{
    /// <summary>
    /// Базовый класс для реализации "умных перечислений" (Smart Enum).
    /// Позволяет объединить простоту enum (доступ по имени и по ключу)
    /// с возможностью иметь собственное поведение (методы) у каждого значения.
    /// </summary>
    /// <typeparam name="TEnum">Тип семейства перечисления (наследник самого себя)</typeparam>
    public abstract class Enumeration<TEnum> : IEquatable<Enumeration<TEnum>>
        where TEnum : Enumeration<TEnum>
    {
        private static readonly Dictionary<int, Func<TEnum>> _keyFactories = FetchKeyFactories();
        private static readonly Dictionary<string, Func<TEnum>> _nameFactories = FetchNameFactories();

        public int Key { get; }
        public string Name { get; }

        protected Enumeration(int key, string name)
        {
            Key = key;
            Name = name;
        }

        public static TEnum FromKey(int key)
        {
            return _keyFactories.TryGetValue(key, out Func<TEnum>? factory)
                ? factory()
                : throw new ArgumentException($"Не поддерживаемый ключ перечисления - {key}");
        }

        public static TEnum FromName(string name)
        {
            return _nameFactories.TryGetValue(name, out Func<TEnum>? factory)
                ? factory()
                : throw new ArgumentException($"Не поддерживаемое название перечисления - {name}");
        }

        public static IReadOnlyCollection<TEnum> GetAll()
        {
            return _keyFactories.Values.Select(factory => factory()).ToList();
        }

        private static Dictionary<int, Func<TEnum>> FetchKeyFactories()
        {
            var factories = new Dictionary<int, Func<TEnum>>();

            foreach (Type entry in FetchTypes())
            {
                Func<TEnum> factory = CreateFactoryFromConstructor(entry);
                TEnum enumeration = factory();
                factories.Add(enumeration.Key, factory);
            }

            return factories;
        }

        private static Dictionary<string, Func<TEnum>> FetchNameFactories()
        {
            var factories = new Dictionary<string, Func<TEnum>>();

            foreach (Type entry in FetchTypes())
            {
                Func<TEnum> factory = CreateFactoryFromConstructor(entry);
                TEnum enumeration = factory();
                factories.Add(enumeration.Name, factory);
            }

            return factories;
        }

        private static Func<TEnum> CreateFactoryFromConstructor(Type type)
        {
            ConstructorInfo constructor = type
                .GetConstructors()
                .First(c => c.GetParameters().Length == 0);

            return () => (TEnum)constructor.Invoke(null);
        }

        private static IEnumerable<Type> FetchTypes()
        {
            Type enumType = typeof(TEnum);

            return enumType
                .Assembly.GetTypes()
                .Where(t => t.IsSubclassOf(enumType) && !t.IsAbstract);
        }

        public bool Equals(Enumeration<TEnum>? other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return GetType() == other.GetType() && Key == other.Key;
        }

        public override bool Equals(object? obj) => Equals(obj as Enumeration<TEnum>);

        public override int GetHashCode() => HashCode.Combine(GetType(), Key);

        public static bool operator ==(Enumeration<TEnum>? left, Enumeration<TEnum>? right)
        {
            if (left is null)
                return right is null;

            return left.Equals(right);
        }

        public static bool operator !=(Enumeration<TEnum>? left, Enumeration<TEnum>? right) => !(left == right);

        public override string ToString() => Name;
    }
}
