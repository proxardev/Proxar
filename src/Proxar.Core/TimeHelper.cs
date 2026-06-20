/*
 * Copyright 2026 Proxar
 * SPDX-License-Identifier: Apache-2.0
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */


using System.Globalization;


namespace Proxar.Utilities;

/// <summary>
/// 时间工具类，提供时间戳获取、时间格式转换等静态方法。
/// </summary>
public static class TimeHelper
{
    /// <summary>
    /// 获取当前 UTC 时间的毫秒级时间戳。
    /// </summary>
    /// <returns>当前 UTC 时间的毫秒数。</returns>
    public static long GetMSSecond()
    {
        DateTimeOffset currentTime2 = DateTimeOffset.UtcNow;
        long timestampInSeconds2 = currentTime2.ToUnixTimeMilliseconds();
        return timestampInSeconds2;
    }

    /// <summary>
    /// 获取当前 UTC 时间的秒级时间戳。
    /// </summary>
    /// <returns>当前 UTC 时间的秒数。</returns>
    public static int GetSecond()
    {
        DateTimeOffset currentTime2 = DateTimeOffset.UtcNow;
        long timestampInSeconds2 = currentTime2.ToUnixTimeMilliseconds();
        return (int)(timestampInSeconds2 / 1000);
    }

    /// <summary>
    /// 将秒级时间戳转换为本地时间的格式化字符串（yyyy-MM-dd HH:mm:ss）。
    /// </summary>
    /// <param name="second">秒级时间戳，为 0 时使用当前时间。</param>
    /// <returns>格式化后的时间字符串。</returns>
    public static string TimeStr(int second = 0)
    {
        if (second == 0)
        {
            second = GetSecond();
        }
        var date = DateTimeOffset.FromUnixTimeSeconds(second).LocalDateTime;
        return date.ToString("yyyy-MM-dd HH:mm:ss");
    }

    /// <summary>
    /// 将毫秒级时间戳转换为本地时间的格式化字符串（yyyy-MM-dd HH:mm:ss.ffff）。
    /// </summary>
    /// <param name="milliSecond">毫秒级时间戳，为 0 时使用当前时间。</param>
    /// <returns>格式化后的时间字符串。</returns>
    public static string MilliTimeStr(long milliSecond = 0)
    {
        if (milliSecond == 0)
        {
            milliSecond = GetMSSecond();
        }
        var date = DateTimeOffset.FromUnixTimeMilliseconds(milliSecond).LocalDateTime;
        return date.ToString("yyyy-MM-dd HH:mm:ss.ffff");
    }

    /// <summary>
    /// 将指定格式的时间字符串转换为秒级时间戳。
    /// </summary>
    /// <param name="timeStr">时间字符串，格式：yyyy-MM-dd HH:mm:ss。</param>
    /// <returns>对应的秒级时间戳。</returns>
    /// <exception cref="ArgumentException">当 <paramref name="timeStr"/> 为 null 或空白时抛出。</exception>
    /// <exception cref="FormatException">当 <paramref name="timeStr"/> 格式不正确时抛出。</exception>
    public static long GetSecondByStr(string timeStr)
    {
        if (string.IsNullOrWhiteSpace(timeStr))
            throw new ArgumentException("时间字符串不能为空");

        try
        {
            var dateTime = DateTimeOffset.ParseExact(
                timeStr,
                "yyyy-MM-dd HH:mm:ss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None);
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(dateTime.ToUniversalTime() - epoch).TotalSeconds;
        }
        catch (FormatException ex)
        {
            throw new FormatException($"时间字符串格式错误，应为 'yyyy-MM-dd HH:mm:ss'，实际为 '{timeStr}'", ex);
        }
    }
}