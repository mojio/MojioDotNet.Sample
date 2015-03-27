using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Mojio.Events;

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
                        return "Diagnostics";
                        break;
                    case EventType.Diagnostic:
                        var h = (_event as DiagnosticEvent);
                        return "Deceleration: " + h.DTCs.ToString();
                        break;
                    case EventType.FenceEntered:
                        var i = (_event as FenceEvent);
                        return "Deceleration: " + i.Deceleration;
                        break;
                    case EventType.FenceExited:
                        var j = (_event as FenceEvent);
                        return "Deceleration: " + j.Deceleration;
                        break;
                    case EventType.HardAcceleration:
                        var k = (_event as AccelerationEvent);
                        return "Deceleration: " + k.Deceleration;
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
                        return "Deceleration: " + p.Deceleration;
                        break;
                    case EventType.IdleEvent:
                        var q = (_event as IdleEvent);
                        return "Deceleration: " + q.Deceleration;
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
                        return "Deceleration: " + w.Deceleration;
                        break;
                    case EventType.MILWarning:
                        return "MIL Warning";
                        break;
                    case EventType.Message:
                        return "Message: ";
                        break;
                    case EventType.Mileage:
                        var z = (_event as MileageEvent);
                        return "Deceleration: " + z.Deceleration;
                        break;
                    case EventType.MojioIdle:
                        var aa = (_event as IdleEvent);
                        return "Deceleration: " + aa.Deceleration;
                        break;
                    case EventType.MojioOff:
                        var bb = (_event as OffStatusEvent);
                        return "Deceleration: " + bb.Deceleration;
                        break;
                    case EventType.MojioOn:
                        var cc = (_event as OffStatusEvent);
                        return "Deceleration: " + cc.Deceleration;
                        break;
                    case EventType.MojioWake:
                        var dd = (_event as AccelerationEvent);
                        return "Deceleration: " + dd.Deceleration;
                        break;
                    case EventType.MovementStart:
                        var ee = (_event as MovementEvent);
                        return "Deceleration: " + ee.Deceleration;
                        break;
                    case EventType.MovementStop:
                        var ff = (_event as MovementEvent);
                        return "Deceleration: " + ff.Deceleration;
                        break;
                    case EventType.OffStatus:
                        var gg = (_event as OffStatusEvent);
                        return "Deceleration: " + gg.Deceleration;
                        break;
                    case EventType.Park:
                        var hh = (_event as ParkEvent);
                        return "Deceleration: " + hh.Deceleration;
                        break;
                    case EventType.PreSleepWarning:
                        var ii = (_event as SleepEvent);
                        return "Deceleration: " + ii.Deceleration;
                        break;
                    case EventType.RPM:
                        var jj = (_event as RPMEvent);
                        return "Deceleration: " + jj.Deceleration;
                        break;
                    case EventType.Speed:
                        var kk = (_event as SpeedEvent);
                        return "Deceleration: " + kk.Deceleration;
                        break;
                    case EventType.TowStart:
                        var ll = (_event as TowEvent);
                        return "Deceleration: " + ll.Deceleration;
                        break;
                    case EventType.TowStop:
                        var mm = (_event as TowEvent);
                        return "Deceleration: " + mm.Deceleration;
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
            set { 
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
