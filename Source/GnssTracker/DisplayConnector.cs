﻿using Meadow.Hardware;
using System;
using static Meadow.Devices.DisplayConnector;

namespace Meadow.Devices;

/// <summary>
/// Represents the display connector on GNSS Tracker
/// </summary>
public class DisplayConnector : Connector<DisplayConnectorPinDefinitions>
{
    /// <summary>
    /// The set of Display connector connector pins
    /// </summary>
    public static class PinNames
    {
        /// <summary>
        /// Chip Select pin
        /// </summary>
        public const string CS = "CS";
        /// <summary>
        /// Reset pin
        /// </summary>
        public const string RST = "RST";
        /// <summary>
        /// Data/Command pin
        /// </summary>
        public const string DC = "DC";
        /// <summary>
        /// Busy status pin
        /// </summary>
        public const string BUSY = "BUSY";
        /// <summary>
        /// SPI Clock pin
        /// </summary>
        public const string CLK = "CLK";
        /// <summary>
        /// SPI controller out, peripheral in pin
        /// </summary>
        public const string COPI = "COPI";
        /// <summary>
        /// SPI controller im, peripheral out pin
        /// </summary>
        public const string CIPO = "CIPO";
    }

    /// <summary>
    /// Represents the pins definitions for the Display connector on GNSS Tracker
    /// </summary>
    public class DisplayConnectorPinDefinitions : PinDefinitionBase
    {
        private readonly IPin? _cs;
        private readonly IPin? _rst;
        private readonly IPin? _dc;
        private readonly IPin? _busy;
        private readonly IPin? _clk;
        private readonly IPin? _copi;
        private readonly IPin? _cipo;

        /// <summary>
        /// Chip Select pin
        /// </summary>
        public IPin CS => _cs ?? throw new PlatformNotSupportedException("Pin not connected");
        /// <summary>
        /// Reset pin
        /// </summary>
        public IPin RST => _rst ?? throw new PlatformNotSupportedException("Pin not connected");
        /// <summary>
        /// Data/Command pin
        /// </summary>
        public IPin DC => _dc ?? throw new PlatformNotSupportedException("Pin not connected");
        /// <summary>
        /// Busy status pin
        /// </summary>
        public IPin BUSY => _busy ?? throw new PlatformNotSupportedException("Pin not connected");
        /// <summary>
        /// SPI Clock pin
        /// </summary>
        public IPin CLK => _clk ?? throw new PlatformNotSupportedException("Pin not connected");
        /// <summary>
        /// SPI controller out, peripheral in pin
        /// </summary>
        public IPin COPI => _copi ?? throw new PlatformNotSupportedException("Pin not connected");
        /// <summary>
        /// SPI controller in, peripheral out pin
        /// </summary>
        public IPin CIPO => _cipo ?? throw new PlatformNotSupportedException("Pin not connected");

        internal DisplayConnectorPinDefinitions(PinMapping mapping)
        {
            foreach (var m in mapping)
            {
                switch (m.PinName)
                {
                    case PinNames.CS:
                        _cs = m.ConnectsTo;
                        break;
                    case PinNames.RST:
                        _rst = m.ConnectsTo;
                        break;
                    case PinNames.DC:
                        _dc = m.ConnectsTo;
                        break;
                    case PinNames.BUSY:
                        _busy = m.ConnectsTo;
                        break;
                    case PinNames.CLK:
                        _clk = m.ConnectsTo;
                        break;
                    case PinNames.COPI:
                        _copi = m.ConnectsTo;
                        break;
                    case PinNames.CIPO:
                        _cipo = m.ConnectsTo;
                        break;
                }
            }
        }
    }

    /// <param name="name">The connector name</param>
    /// <param name="mapping">The mappings to the host controller</param>
    public DisplayConnector(string name, PinMapping mapping)
        : base(name, new DisplayConnectorPinDefinitions(mapping))
    { }
}