// Copyright (c) 2026 PBLivingston
// SPDX-License-Identifier: MPL-2.0

namespace PBLivingston.VoicemeeterAPI;

#if NET6_0_OR_GREATER
using System.Globalization;
#endif

using System.Text;

internal static class StringBuilderExt
{
    public static StringBuilder AddArg<T>(this StringBuilder builder, string label, T value) where T : struct
    {
#if NET6_0_OR_GREATER
        builder.Append(CultureInfo.InvariantCulture, $"{label}: {value}; ");
#else
        builder.Append($"{label}: {value}; ");
#endif
        return builder;
    }

    public static StringBuilder AddNullableArg<T>(this StringBuilder builder, string label, T? value)
    {
#if NET6_0_OR_GREATER
        if (value is not null)
        {
            builder.Append(CultureInfo.InvariantCulture, $"{label}: {value}; ");
        }
#else
        if (!EqualityComparer<T?>.Default.Equals(value, default))
        {
            builder.Append($"{label}: {value}; ");
        }
#endif

        return builder;
    }
}

internal static class SemaphoreExt
{
    public static async Task<Scope> EnterScopeAsync(this SemaphoreSlim semaphore, CancellationToken cancellationToken = default)
    {
        await semaphore.WaitAsync(cancellationToken);
        return new Scope(semaphore);
    }

    public static Scope EnterScope(this SemaphoreSlim semaphore)
    {
        semaphore.Wait();
        return new Scope(semaphore);
    }

    public readonly struct Scope(SemaphoreSlim semaphore) : IDisposable
    {
        private readonly SemaphoreSlim semaphore = semaphore;
        public void Dispose()
            => this.semaphore?.Release();
    }
}

#if NET9_0_OR_GREATER
#else
internal static class LockExt
{
    public static Scope EnterScope(this LockObject lockObject)
        => new(lockObject);

    public readonly ref struct Scope : IDisposable
    {
        private readonly LockObject lockObject;

        public Scope(LockObject lockObject)
        {
            this.lockObject = lockObject;
            Monitor.Enter(this.lockObject);
        }

        public void Dispose()
            => Monitor.Exit(this.lockObject);
    }
}
#endif
