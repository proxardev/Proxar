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


public static class TimeHelper
{

    public static long GetMSSecond()
    {
        DateTimeOffset currentTime2 = DateTimeOffset.UtcNow;

        // 计算时间戳（秒）
        long timestampInSeconds2 = currentTime2.ToUnixTimeMilliseconds();
        return timestampInSeconds2;
    }

    public static int GetSecond()
    {
        DateTimeOffset currentTime2 = DateTimeOffset.UtcNow;

        // 计算时间戳（秒）
        long timestampInSeconds2 = currentTime2.ToUnixTimeMilliseconds();
        return (int)(timestampInSeconds2 / 1000);
    }

    public static string TimeStr(int second = 0)
    {
        if (second == 0)
        {
            second = GetSecond();
        }
        var date = DateTimeOffset.FromUnixTimeSeconds(second).LocalDateTime;
        return date.ToString("yyyy-MM-dd HH:mm:ss");
    }

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
    /// 将时间字符串转换为时间戳（秒）
    /// </summary>
    /// <param name="timeStr">时间字符串，格式：yyyy-MM-dd HH:mm:ss</param>
    /// <returns>时间戳（秒）</returns>
    public static long GetSecondByStr(string timeStr)
    {
        if (string.IsNullOrWhiteSpace(timeStr))
            throw new ArgumentException("时间字符串不能为空");

        try
        {

            // 解析时间字符串
            var dateTime = DateTimeOffset.ParseExact(
                timeStr,
                "yyyy-MM-dd HH:mm:ss",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None);
            // 转换为UTC时间戳（秒）
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return (long)(dateTime.ToUniversalTime() - epoch).TotalSeconds;
        }
        catch (FormatException ex)
        {
            throw new FormatException($"时间字符串格式错误，应为 'yyyy-MM-dd HH:mm:ss'，实际为 '{timeStr}'", ex);
        }
    }
}