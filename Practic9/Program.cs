using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Practic9
{
    abstract class Storage
    {
        public string Name { get; set; }
        public string Model { get; set; }

        public abstract double GetMemoryCapacity();
        public abstract double CopyData(double dataSize);
        public abstract double GetFreeMemory();
        public abstract string GetDeviceInfo();
    }

    class Flash : Storage
    {
        public double Speed { get; set; } // MB/s
        public double MemoryCapacity { get; private set; } // GB

        public Flash(double speed, double memoryCapacity)
        {
            Speed = speed;
            MemoryCapacity = memoryCapacity;
        }

        public override double GetMemoryCapacity() => MemoryCapacity;

        public override double CopyData(double dataSize)
        {
            double time = dataSize / Speed / 3600; // Время в часах
            MemoryCapacity -= dataSize / 1024; // Уменьшение свободной памяти
            return time;
        }

        public override double GetFreeMemory() => MemoryCapacity;

        public override string GetDeviceInfo()
        {
            return $"Flash: {MemoryCapacity} GB, Speed: {Speed} MB/s";
        }
    }

    class DVD : Storage
    {
        public double Speed { get; set; } // MB/h
        public double MemoryCapacity { get; private set; } // GB
        public bool IsDoubleSided { get; set; }

        public DVD(double speed, bool isDoubleSided)
        {
            Speed = speed;
            MemoryCapacity = isDoubleSided ? 9 : 4.7;
        }

        public override double GetMemoryCapacity() => MemoryCapacity;

        public override double CopyData(double dataSize)
        {
            double time = dataSize / Speed; // Время в часах
            MemoryCapacity -= dataSize / 1024; // Уменьшение свободной памяти
            return time;
        }

        public override double GetFreeMemory() => MemoryCapacity;

        public override string GetDeviceInfo()
        {
            string type = IsDoubleSided ? "Double-Sided" : "Single-Sided";
            return $"DVD {type}: {MemoryCapacity} GB, Speed: {Speed} MB/h";
        }
    }

    class HDD : Storage
    {
        public double Speed { get; set; } // MB/s
        public int PartitionsCount { get; set; }
        public double PartitionSize { get; set; } // GB
        private double totalMemoryCapacity;

        public HDD(double speed, int partitionsCount, double partitionSize)
        {
            Speed = speed;
            PartitionsCount = partitionsCount;
            PartitionSize = partitionSize;
            totalMemoryCapacity = partitionsCount * partitionSize;
        }

        public override double GetMemoryCapacity() => totalMemoryCapacity;

        public override double CopyData(double dataSize)
        {
            double time = dataSize / Speed / 3600; // Время в часах
            totalMemoryCapacity -= dataSize / 1024; // Уменьшение свободной памяти
            return time;
        }

        public override double GetFreeMemory() => totalMemoryCapacity;

        public override string GetDeviceInfo()
        {
            return $"HDD: {totalMemoryCapacity} GB, {PartitionsCount} partitions, Speed: {Speed} MB/s";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            // Инициализация устройств
            List<Storage> storages = new List<Storage>
            {
                new Flash(100, 128), // 128 GB Flash with 100 MB/s speed
                new DVD(56, true),   // Double-sided DVD with 56 MB/h speed
                new HDD(60, 4, 500)  // HDD with 4 partitions, 500 GB each, and 60 MB/s speed
            };

            double totalDataSize = 565; // Общий размер данных в ГБ
            double fileSize = 780 / 1024.0; // Размер файла в ГБ (780 МБ)

            CalculateBackup(storages, totalDataSize, fileSize);
        }

        static void CalculateBackup(List<Storage> storages, double totalDataSize, double fileSize)
        {
            double totalMemoryCapacity = 0;
            double totalTime = 0;
            int totalDevicesNeeded = 0;

            foreach (var storage in storages)
            {
                totalMemoryCapacity += storage.GetMemoryCapacity();

                int devicesNeeded = (int)Math.Ceiling(totalDataSize / storage.GetMemoryCapacity());
                double timeForDevice = storage.CopyData(fileSize);
                totalTime += timeForDevice * devicesNeeded;

                totalDevicesNeeded += devicesNeeded;

                Console.WriteLine(storage.GetDeviceInfo());
                Console.WriteLine($"Devices needed: {devicesNeeded}, Total time (hours): {timeForDevice * devicesNeeded}");
                Console.WriteLine();
            }

            Console.WriteLine($"Total Memory Capacity of All Devices: {totalMemoryCapacity} GB");
            Console.WriteLine($"Total Devices Needed: {totalDevicesNeeded}");
            Console.WriteLine($"Total Time for Backup (hours): {totalTime}");
        }
    }
}