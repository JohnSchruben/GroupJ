using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Graphics;
using Microsoft.Maui.Maps.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeSkate.Mobile.Platforms.Android
{
    public class CustomMapHandler : MapHandler
    {
        private const int width = 60;
        private const int height = 92;

        private readonly Dictionary<string, BitmapDescriptor> _iconMap = [];

        public static new IPropertyMapper<MapEx, CustomMapHandler> Mapper = new PropertyMapper<MapEx, CustomMapHandler>(MapHandler.Mapper)
        {
            [nameof(MapEx.CustomPins)] = MapPins
        };

        public Dictionary<string, (Marker Marker, MapPin Pin)> MarkerMap { get; } = [];

        public CustomMapHandler()
            : base(Mapper)
        {
        }

        protected override void ConnectHandler(MapView platformView)
        {
            base.ConnectHandler(platformView);
            var mapReady = new MapCallbackHandler(this);

            PlatformView.GetMapAsync(mapReady);
        }

        private static new void MapPins(IMapHandler handler, Microsoft.Maui.Maps.IMap map)
        {
            if (handler.Map is null || handler.MauiContext is null)
            {
                return;
            }

            if (handler is CustomMapHandler mapHandler)
            {
                foreach (var marker in mapHandler.MarkerMap)
                {
                    marker.Value.Marker.Remove();
                }

                mapHandler.MarkerMap.Clear();

                mapHandler.AddPins();
            }
        }

        private BitmapDescriptor GetIcon(string icon)
        {
            if (_iconMap.TryGetValue(icon, out BitmapDescriptor? value))
            {
                return value;
            }

            var drawable = Context.Resources.GetIdentifier(icon, "drawable", Context.PackageName);
            var bitmap = BitmapFactory.DecodeResource(Context.Resources, drawable);
            var scaled = Bitmap.CreateScaledBitmap(bitmap, width, height, false);
            bitmap.Recycle();
            var descriptor = BitmapDescriptorFactory.FromBitmap(scaled);

            _iconMap[icon] = descriptor;

            return descriptor;
        }

        private void AddPins()
        {
            if (VirtualView is MapEx mapEx && mapEx.CustomPins != null)
            {
                mapEx.CustomPins.CollectionChanged += this.CustomPins_CollectionChanged;
                foreach (var pin in mapEx.CustomPins)
                {
                    UpdateMarker(pin);
                }
            }
        }
        private void UpdateMarker(MapPin pin)
        {
            var markerOption = new MarkerOptions();
            markerOption.SetTitle(pin.Model.SeverityDescription() + " (click to edit)");
            markerOption.SetIcon(GetIcon(pin.Icon));
            markerOption.SetPosition(new LatLng(pin.Position.Latitude, pin.Position.Longitude));
            var marker = Map.AddMarker(markerOption);
            pinMarkerIdMap.Add(pin.Id, marker.Id);
            MarkerMap.Add(marker.Id, (marker, pin));
        }
        private Dictionary<string, string> pinMarkerIdMap = new Dictionary<string, string>();
        private void CustomPins_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (MapPin pin in e.NewItems)
                {
                    UpdateMarker(pin);
                }
            }
            if (e.OldItems != null)
            {
                foreach (MapPin pin in e.OldItems)
                {
                    var markerid = pinMarkerIdMap[pin.Id];
                    if (MarkerMap.TryGetValue(markerid, out (Marker Marker, MapPin Pin) markerTuple))
                    {
                        markerTuple.Marker.Remove();

                        MarkerMap.Remove(markerid);
                        pinMarkerIdMap.Remove(pin.Id);   
                    }
                }
            }
        }

        public void MarkerClick(object sender, GoogleMap.MarkerClickEventArgs args)
        {
            if (MarkerMap.TryGetValue(args.Marker.Id, out (Marker Marker, MapPin Pin) value))
            {
                value.Marker.ShowInfoWindow();
            }
        }

        public void InfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs args)
        {
            if (MarkerMap.TryGetValue(args.Marker.Id, out (Marker Marker, MapPin Pin) value))
            {
                value.Pin.EditCommand?.Execute(null);
            }
        }
    }

    public class MapCallbackHandler : Java.Lang.Object, IOnMapReadyCallback
    {
        private readonly CustomMapHandler mapHandler;

        public MapCallbackHandler(CustomMapHandler mapHandler)
        {
            this.mapHandler = mapHandler;
        }

        public void OnMapReady(GoogleMap googleMap)
        {
            mapHandler.UpdateValue(nameof(MapEx.CustomPins));
            googleMap.MarkerClick += mapHandler.MarkerClick;
            googleMap.InfoWindowClick += mapHandler.InfoWindowClick;
        }
    }
}
