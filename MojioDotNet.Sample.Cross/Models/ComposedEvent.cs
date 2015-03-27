using Mojio.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MojioDotNet.Sample.Cross.Models
{
    public class ComposedEvent : INotifyPropertyChanged
    {
        public string Display
        {
            get
            {
                switch (_event.EventType)
                {
                    case EventType.Acceleration:
                        var a = (_event as AccelerationEvent);
                        return "Acceleration: " + a.Acceleration.ToString();
                        break;

                    case EventType.Accelerometer:
                        var b = (_event as AccelerometerEvent);
                        return "Accelerometer: " + b.Accelerometer;
                        break;

                    case EventType.Accident:
                        return "Accident!";
                        break;

                    case EventType.Alert:
                        return "Alert";
                        break;

                    case EventType.ConnectionLost:
                        return "Connection Lost";
                        break;

                    case EventType.Deceleration:
                        var f = (_event as DecelerationEvent);
                        return "Deceleration: " + f.Deceleration;
                        break;

                    case EventType.DeviceDiagnostic:
                        var g = (_event as DeviceDiagnosticEvent);
                        return "Device Diagnostics";
                        break;

                    case EventType.Diagnostic:
                        var h = (_event as DiagnosticEvent);
                        return "Diagnostic: " + h.DTCs.ToString();
                        break;

                    case EventType.FenceEntered:
                        return "Entered a Geo Fence";
                        break;

                    case EventType.FenceExited:
                        return "Exited a Geo Fence "; ;
                        break;

                    case EventType.HardAcceleration:
                        return "Hard Acceleration";
                        break;

                    case EventType.HardBrake:
                        return "Hard Brake!";
                        break;

                    case EventType.HardLeft:
                        return "Hard Left!";
                        break;

                    case EventType.HardRight:
                        return "Hard Right!";
                        break;

                    case EventType.HeadingChange:
                        var o = (_event as HeadingChangeEvent);
                        return "Heading Changed: " + o.Heading;
                        break;

                    case EventType.HeartBeat:
                        var p = (_event as HeartBeatEvent);
                        return "Heart beat";
                        break;

                    case EventType.IdleEvent:
                        return "Idling";
                        break;

                    case EventType.IgnitionOff:
                        return "Ignition Off";
                        break;

                    case EventType.IgnitionOn:
                        return "Ignition On";
                        break;

                    case EventType.Information:
                        return "Information";
                        break;

                    case EventType.Log:
                        return "Log";
                        break;

                    case EventType.LowBattery:
                        var v = (_event as BatteryEvent);
                        return "Battery Voltage: " + v.BatteryVoltage;
                        break;

                    case EventType.LowFuel:
                        var w = (_event as FuelEvent);
                        return "Fuel: " + w.FuelLevel;
                        break;

                    case EventType.MILWarning:
                        return "MIL Warning";
                        break;

                    case EventType.Message:
                        return "Message";
                        break;

                    case EventType.Mileage:
                        return "Mileage";
                        break;

                    case EventType.MojioIdle:
                        return "Mojio Idle";
                        break;

                    case EventType.MojioOff:
                        return "Mojio Off";
                        break;

                    case EventType.MojioOn:
                        return "Mojion On";
                        break;

                    case EventType.MojioWake:
                        return "Mojio Wake";
                        break;

                    case EventType.MovementStart:
                        return "Movement Start";
                        break;

                    case EventType.MovementStop:
                        return "Movement Stop";
                        break;

                    case EventType.OffStatus:
                        return "Off";
                        break;

                    case EventType.Park:
                        return "Parked";
                        break;

                    case EventType.PreSleepWarning:
                        return "Pre Sleep Warning";
                        break;

                    case EventType.RPM:
                        var jj = (_event as RPMEvent);
                        return "RPM: " + jj.RPM;
                        break;

                    case EventType.Speed:
                        var kk = (_event as SpeedEvent);
                        return "Speed: " + kk.Speed;
                        break;

                    case EventType.TowStart:
                        return "Towing Started";
                        break;

                    case EventType.TowStop:
                        return "Towing Stopped: ";
                        break;

                    case EventType.Warning:
                        return "Warning";
                        break;

                    default:
                        return _event.EventType.ToString();
                }
                return _event.EventType.ToString();
            }
        }

        public Guid Id
        {
            get { return _event.Id; }
        }

        public DateTime DateTime
        {
            get { return _event.Time.ToLocalTime(); }
        }

        private Event _event;

        public Event Event
        {
            get { return _event; }
            set
            {
                _event = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}