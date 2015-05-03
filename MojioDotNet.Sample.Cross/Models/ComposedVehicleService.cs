using System;
using System.Collections.Generic;
using System.Globalization;
using Mojio;

namespace MojioDotNet.Sample.Cross.Models
{
    public class ComposedVehicleService
    {
        private VehicleService _vehicleService;

        public ComposedVehicleService()
        {
            SubServices = new List<VehicleService>();
        }

        public VehicleService VehicleService
        {
            get { return _vehicleService; }
            set
            {
                _vehicleService = value;
                if (!this.SubServices.Contains(value))
                {
                    this.SubServices.Add(value);
                }
            }
        }

        public double? CurrentOdometer { get; set; }

        public bool IsTimeBased { get; set; }

        public string Title
        {
            get
            {
                var value = String.Format("{0:#,###,###}", Math.Round(VehicleService.Value, 0));
                return string.Format("{0} {1} {2}", VehicleService.IntervalType, value, VehicleService.Units);
            }
        }

        public List<VehicleService> SubServices { get; set; }

        public string Subtitle
        {
            get
            {
                string result = "";
                foreach (var sub in SubServices)
                {
                    var s = sub.MaintenanceName;

                    if (!string.IsNullOrEmpty(sub.MaintenanceCategory))
                    {
                        s = string.Format("{1} - {0}", s, sub.MaintenanceCategory);
                    }


                    if (!string.IsNullOrEmpty(sub.MaintenanceNotes))
                    {
                        s = string.Format("{0} ({1})", s, sub.MaintenanceNotes);
                    }

                    if (!string.IsNullOrEmpty(sub.ServiceEvent))
                    {
                        s = string.Format("{0} or {1}", s, sub.ServiceEvent);
                    }

                    if (!string.IsNullOrEmpty(sub.OperatingParameter))
                    {
                        s = string.Format("{0} if {1}", s, sub.OperatingParameter);
                    }

                    s = string.Format("· {0}", s);
                    if (string.IsNullOrEmpty(result))
                    {
                        result = s;
                    }
                    else
                    {
                        result = string.Format("{0}\n{1}", result, s);                        
                    }
                }
                return result;

            }
        }
    }
}