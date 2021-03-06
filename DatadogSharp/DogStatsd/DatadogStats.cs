﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DatadogSharp.DogStatsd
{
    public class DatadogStats : IDisposable
    {
        public static DatadogStats Default { get; private set; } = new DatadogStats();

        public static void ConfigureDefault(string address, int port = 8125, string metricNamePrefix = null, string[] defaultTags = null)
        {
            Default.Dispose();
            Default = new DatadogStats(address, port, metricNamePrefix, defaultTags);
        }

        bool isNull;
        string metricNamePrefix = null;
        string[] defaultTags = null;
        IPEndPoint endPoint;
        Socket udpSocket;
        Encoding utf8 = Encoding.UTF8;

        DatadogStats()
        {
            isNull = true;
        }

        public DatadogStats(string address, int port, string metricNamePrefix = null, string[] defaultTags = null)
        {
            this.metricNamePrefix = metricNamePrefix;
            if (defaultTags != null)
            {
                this.defaultTags = defaultTags;
            }

            this.udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            this.endPoint = new IPEndPoint(IPAddress.Parse(address), port);
            this.isNull = false;
        }

        public string WithPrefix(string name)
        {
            if (string.IsNullOrEmpty(metricNamePrefix)) return name;

            return metricNamePrefix + "." + name;
        }

        public string[] WithDefaultTag(string[] tag)
        {
            if (defaultTags == null || defaultTags.Length == 0)
            {
                return tag;
            }
            else
            {
                if (tag == null || tag.Length == 0) return defaultTags;

                var joinedTags = new string[defaultTags.Length + tag.Length];
                Array.Copy(defaultTags, 0, joinedTags, 0, defaultTags.Length);
                Array.Copy(tag, 0, joinedTags, defaultTags.Length, tag.Length);
                return joinedTags;
            }
        }

        public void Send(string command)
        {
            Send(utf8.GetBytes(command));
        }

        public void Send(byte[] command)
        {
            udpSocket.SendTo(command, 0, command.Length, SocketFlags.None, endPoint);
        }

        // Metrics

        public void Increment(string metricName, long value = 1, double sampleRate = 1.0, string[] tags = null)
        {
            Counter(metricName, value, sampleRate, tags);
        }

        public void Decrement(string metricName, long value = 1, double sampleRate = 1.0, string[] tags = null)
        {
            Counter(metricName, -value, sampleRate, tags);
        }

        public void Counter(string metricName, long value, double sampleRate = 1.0, string[] tags = null)
        {
            if (isNull) return;
            if (sampleRate == 1.0 || sampleRate < ThreadSafeUtil.ThreadStaticRandom.NextDouble())
            {
                var command = DogStatsDFormatter.Counter(WithPrefix(metricName), value, sampleRate, WithDefaultTag(tags));
                Send(command);
            }
        }

        public void Gauge(string metricName, long value, double sampleRate = 1.0, string[] tags = null)
        {
            if (isNull) return;
            if (sampleRate == 1.0 || sampleRate < ThreadSafeUtil.ThreadStaticRandom.NextDouble())
            {
                var command = DogStatsDFormatter.Gauge(WithPrefix(metricName), value, sampleRate, WithDefaultTag(tags));
                Send(command);
            }
        }

        public void Gauge(string metricName, double value, double sampleRate = 1.0, string[] tags = null)
        {
            if (isNull) return;
            if (sampleRate == 1.0 || sampleRate < ThreadSafeUtil.ThreadStaticRandom.NextDouble())
            {
                var command = DogStatsDFormatter.Gauge(WithPrefix(metricName), value, sampleRate, WithDefaultTag(tags));
                Send(command);
            }
        }

        public void Histogram(string metricName, long value, double sampleRate = 1.0, string[] tags = null)
        {
            if (isNull) return;
            if (sampleRate == 1.0 || sampleRate < ThreadSafeUtil.ThreadStaticRandom.NextDouble())
            {
                var command = DogStatsDFormatter.Histogram(WithPrefix(metricName), value, sampleRate, WithDefaultTag(tags));
                Send(command);
            }
        }

        public void Histogram(string metricName, double value, double sampleRate = 1.0, string[] tags = null)
        {
            if (isNull) return;
            if (sampleRate == 1.0 || sampleRate < ThreadSafeUtil.ThreadStaticRandom.NextDouble())
            {
                var command = DogStatsDFormatter.Histogram(WithPrefix(metricName), value, sampleRate, WithDefaultTag(tags));
                Send(command);
            }
        }

        public void Timer(string metricName, long value, double sampleRate = 1.0, string[] tags = null)
        {
            if (isNull) return;
            if (sampleRate == 1.0 || sampleRate < ThreadSafeUtil.ThreadStaticRandom.NextDouble())
            {
                var command = DogStatsDFormatter.Timer(WithPrefix(metricName), value, sampleRate, WithDefaultTag(tags));
                Send(command);
            }
        }

        public void Timer(string metricName, double value, double sampleRate = 1.0, string[] tags = null)
        {
            if (isNull) return;
            if (sampleRate == 1.0 || sampleRate < ThreadSafeUtil.ThreadStaticRandom.NextDouble())
            {
                var command = DogStatsDFormatter.Timer(WithPrefix(metricName), value, sampleRate, WithDefaultTag(tags));
                Send(command);
            }
        }

        public void Set(string metricName, long value, double sampleRate = 1.0, string[] tags = null)
        {
            if (isNull) return;
            if (sampleRate == 1.0 || sampleRate < ThreadSafeUtil.ThreadStaticRandom.NextDouble())
            {
                var command = DogStatsDFormatter.Set(WithPrefix(metricName), value, sampleRate, WithDefaultTag(tags));
                Send(command);
            }
        }

        public void Set(string metricName, double value, double sampleRate = 1.0, string[] tags = null)
        {
            if (isNull) return;
            if (sampleRate == 1.0 || sampleRate < ThreadSafeUtil.ThreadStaticRandom.NextDouble())
            {
                var command = DogStatsDFormatter.Set(WithPrefix(metricName), value, sampleRate, WithDefaultTag(tags));
                Send(command);
            }
        }

        public MeasureElapsedScope BeginTimer(string metricName, double sampleRate = 1.0, string[] tags = null)
        {
            return MeasureElapsedScope.StartNew(this, MeasureType.Timer, metricName, sampleRate, tags);
        }

        public MeasureElapsedScope BeginGauge(string metricName, double sampleRate = 1.0, string[] tags = null)
        {
            return MeasureElapsedScope.StartNew(this, MeasureType.Gauge, metricName, sampleRate, tags);
        }

        public MeasureElapsedScope BeginHistogram(string metricName, double sampleRate = 1.0, string[] tags = null)
        {
            return MeasureElapsedScope.StartNew(this, MeasureType.Histogram, metricName, sampleRate, tags);
        }

        public CounterScope BeginCounter(string metricName, long value = 1, string[] tags = null)
        {
            return CounterScope.StartNew(this, metricName, value, tags);
        }

        public void Event(string title, string text, int? dateHappened = null, string hostName = null, string aggregationKey = null, Priority priority = Priority.Normal, string sourceTypeName = null, AlertType alertType = AlertType.Info, string[] tags = null, bool truncateText = true)
        {
            if (isNull) return;

            var command = DogStatsDFormatter.Event(title, text, dateHappened, hostName, aggregationKey, priority, sourceTypeName, alertType, WithDefaultTag(tags), truncateText);
            Send(command);
        }

        public void ServiceCheck(string name, string status, int? timestamp = null, string hostName = null, string[] tags = null, string serviceCheckMessage = null, bool truncateText = true)
        {
            if (isNull) return;

            var command = DogStatsDFormatter.ServiceCheck(name, status, timestamp, hostName, WithDefaultTag(tags), serviceCheckMessage, truncateText);
            Send(command);
        }

        public void Dispose()
        {
            if (isNull) return;
            this.udpSocket.Dispose();
            isNull = true;
        }
    }

    public struct MeasureElapsedScope : IDisposable
    {
        System.Diagnostics.Stopwatch sw;
        DatadogStats ds;
        MeasureType type;
        string metricName;
        double sampleRate;
        string[] tags;

        public static MeasureElapsedScope StartNew(DatadogStats dogstats, MeasureType type, string metricName, double sampleRate, string[] tags)
        {
            return new MeasureElapsedScope { sw = ThreadSafeUtil.RentStopwatchStartNew(), ds = dogstats, type = type, metricName = metricName, tags = tags, sampleRate = sampleRate };
        }

        public void Dispose()
        {
            if (sw != null)
            {
                sw.Stop();

                switch (type)
                {
                    case MeasureType.Timer:
                        ds.Timer(metricName, sw.Elapsed.TotalMilliseconds, sampleRate, tags);
                        break;
                    case MeasureType.Gauge:
                        ds.Gauge(metricName, sw.Elapsed.TotalMilliseconds, sampleRate, tags);
                        break;
                    case MeasureType.Histogram:
                        ds.Histogram(metricName, sw.Elapsed.TotalMilliseconds, sampleRate, tags);
                        break;
                    default:
                        break;
                }

                ThreadSafeUtil.ReturnStopwatch(sw);
                sw = null;
                ds = null;
            }
        }
    }

    public enum MeasureType
    {
        Timer, Gauge, Histogram
    }

    public struct CounterScope : IDisposable
    {
        DatadogStats ds;
        string metricName;
        long value;
        string[] tags;

        public static CounterScope StartNew(DatadogStats dogstats, string metricName, long value, string[] tags)
        {
            dogstats.Counter(metricName, value, tags: tags);
            return new CounterScope { ds = dogstats, metricName = metricName, tags = tags };
        }

        public void Dispose()
        {
            if (ds != null)
            {
                ds.Counter(metricName, -value, tags: tags);
                ds = null;
            }
        }
    }
}