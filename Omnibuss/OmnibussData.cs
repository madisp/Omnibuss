#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.261
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;


using System.IO;
using System.IO.IsolatedStorage;
using Microsoft.Phone.Data.Linq.Mapping;
using Microsoft.Phone.Data.Linq;


public class DebugWriter : TextWriter
{
    private const int DefaultBufferSize = 256;
    private System.Text.StringBuilder _buffer;

    public DebugWriter()
    {
        BufferSize = 256;
        _buffer = new System.Text.StringBuilder(BufferSize);
    }

    public int BufferSize
    {
        get;
        private set;
    }

    public override System.Text.Encoding Encoding
    {
        get { return System.Text.Encoding.UTF8; }
    }

    #region StreamWriter Overrides
    public override void Write(char value)
    {
        _buffer.Append(value);
        if (_buffer.Length >= BufferSize)
            Flush();
    }

    public override void WriteLine(string value)
    {
        Flush();

        using(var reader = new StringReader(value))
        {
            string line; 
            while( null != (line = reader.ReadLine()))
                System.Diagnostics.Debug.WriteLine(line);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            Flush();
    }

    public override void Flush()
    {
        if (_buffer.Length > 0)
        {
            System.Diagnostics.Debug.WriteLine(_buffer);
            _buffer.Clear();
        }
    }
    #endregion
}


public partial class OmnibussDataContext : System.Data.Linq.DataContext
{
	
	public bool CreateIfNotExists()
	{
		bool created = false;
		using (var db = new OmnibussDataContext(OmnibussDataContext.ConnectionString))
		{
			if (!db.DatabaseExists())
			{
				string[] names = this.GetType().Assembly.GetManifestResourceNames();
				string name = names.Where(n => n.EndsWith(FileName)).FirstOrDefault();
				if (name != null)
				{
					using (Stream resourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(name))
					{
						if (resourceStream != null)
						{
							using (IsolatedStorageFile myIsolatedStorage = IsolatedStorageFile.GetUserStoreForApplication())
							{
								using (IsolatedStorageFileStream fileStream = new IsolatedStorageFileStream(FileName, FileMode.Create, myIsolatedStorage))
								{
									using (BinaryWriter writer = new BinaryWriter(fileStream))
									{
										long length = resourceStream.Length;
										byte[] buffer = new byte[32];
										int readCount = 0;
										using (BinaryReader reader = new BinaryReader(resourceStream))
										{
											// read file in chunks in order to reduce memory consumption and increase performance
											while (readCount < length)
											{
												int actual = reader.Read(buffer, 0, buffer.Length);
												readCount += actual;
												writer.Write(buffer, 0, actual);
											}
										}
									}
								}
							}
							created = true;
						}
						else
						{
							db.CreateDatabase();
							created = true;
						}
					}
				}
				else
				{
					db.CreateDatabase();
					created = true;
				}
			}
		}
		return created;
	}
	
	public bool LogDebug
	{
		set
		{
			if (value)
			{
				this.Log = new DebugWriter();
			}
		}
	}
	
	public static string ConnectionString = "Data Source=isostore:/Omnibuss.sdf";

	public static string ConnectionStringReadOnly = "Data Source=appdata:/Omnibuss.sdf;File Mode=Read Only;";

	public static string FileName = "Omnibuss.sdf";

	public OmnibussDataContext(string connectionString) : base(connectionString)
	{
		OnCreated();
	}
	
  #region Extensibility Method Definitions
  partial void OnCreated();
  partial void InsertRoute(Route instance);
  partial void UpdateRoute(Route instance);
  partial void DeleteRoute(Route instance);
  partial void InsertService(Service instance);
  partial void UpdateService(Service instance);
  partial void DeleteService(Service instance);
  partial void InsertStop(Stop instance);
  partial void UpdateStop(Stop instance);
  partial void DeleteStop(Stop instance);
  partial void InsertTrip(Trip instance);
  partial void UpdateTrip(Trip instance);
  partial void DeleteTrip(Trip instance);
  #endregion
	
	public System.Data.Linq.Table<Route> Routes
	{
		get
		{
			return this.GetTable<Route>();
		}
	}
	
	public System.Data.Linq.Table<Service> Services
	{
		get
		{
			return this.GetTable<Service>();
		}
	}
	
	public System.Data.Linq.Table<Stop_time> Stop_times
	{
		get
		{
			return this.GetTable<Stop_time>();
		}
	}
	
	public System.Data.Linq.Table<Stop> Stops
	{
		get
		{
			return this.GetTable<Stop>();
		}
	}
	
	public System.Data.Linq.Table<Trip> Trips
	{
		get
		{
			return this.GetTable<Trip>();
		}
	}
}

[Index(Name="UQ__routes__00000000000000D2", Columns="Route_id ASC", IsUnique=true)]
[global::System.Data.Linq.Mapping.TableAttribute(Name="routes")]
public partial class Route : INotifyPropertyChanging, INotifyPropertyChanged
{
	
	private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
	
	private int _Route_id;
	
	private string _Route_short_name;
	
	private string _Route_long_name;
	
	private System.Nullable<int> _Route_type;
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnRoute_idChanging(int value);
    partial void OnRoute_idChanged();
    partial void OnRoute_short_nameChanging(string value);
    partial void OnRoute_short_nameChanged();
    partial void OnRoute_long_nameChanging(string value);
    partial void OnRoute_long_nameChanged();
    partial void OnRoute_typeChanging(System.Nullable<int> value);
    partial void OnRoute_typeChanged();
    #endregion
	
	public Route()
	{
		OnCreated();
	}
	
	[global::System.Data.Linq.Mapping.ColumnAttribute(Name="route_id", Storage="_Route_id", DbType="Int NOT NULL", IsPrimaryKey=true)]
	public int Route_id
	{
		get
		{
			return this._Route_id;
		}
		set
		{
			if ((this._Route_id != value))
			{
				this.OnRoute_idChanging(value);
				this.SendPropertyChanging();
				this._Route_id = value;
				this.SendPropertyChanged("Route_id");
				this.OnRoute_idChanged();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.ColumnAttribute(Name="route_short_name", Storage="_Route_short_name", DbType="NVarChar(64)")]
	public string Route_short_name
	{
		get
		{
			return this._Route_short_name;
		}
		set
		{
			if ((this._Route_short_name != value))
			{
				this.OnRoute_short_nameChanging(value);
				this.SendPropertyChanging();
				this._Route_short_name = value;
				this.SendPropertyChanged("Route_short_name");
				this.OnRoute_short_nameChanged();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.ColumnAttribute(Name="route_long_name", Storage="_Route_long_name", DbType="NVarChar(256)")]
	public string Route_long_name
	{
		get
		{
			return this._Route_long_name;
		}
		set
		{
			if ((this._Route_long_name != value))
			{
				this.OnRoute_long_nameChanging(value);
				this.SendPropertyChanging();
				this._Route_long_name = value;
				this.SendPropertyChanged("Route_long_name");
				this.OnRoute_long_nameChanged();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.ColumnAttribute(Name="route_type", Storage="_Route_type", DbType="Int")]
	public System.Nullable<int> Route_type
	{
		get
		{
			return this._Route_type;
		}
		set
		{
			if ((this._Route_type != value))
			{
				this.OnRoute_typeChanging(value);
				this.SendPropertyChanging();
				this._Route_type = value;
				this.SendPropertyChanged("Route_type");
				this.OnRoute_typeChanged();
			}
		}
	}
	
	public event PropertyChangingEventHandler PropertyChanging;
	
	public event PropertyChangedEventHandler PropertyChanged;
	
	protected virtual void SendPropertyChanging()
	{
		if ((this.PropertyChanging != null))
		{
			this.PropertyChanging(this, emptyChangingEventArgs);
		}
	}
	
	protected virtual void SendPropertyChanged(String propertyName)
	{
		if ((this.PropertyChanged != null))
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}

[Index(Name="UQ__services__00000000000000F0", Columns="Service_id ASC", IsUnique=true)]
[global::System.Data.Linq.Mapping.TableAttribute(Name="services")]
public partial class Service : INotifyPropertyChanging, INotifyPropertyChanged
{
	
	private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
	
	private int _Service_id;
	
	private System.Nullable<byte> _Monday;
	
	private System.Nullable<byte> _Tuesday;
	
	private System.Nullable<byte> _Wednesday;
	
	private System.Nullable<byte> _Thursday;
	
	private System.Nullable<byte> _Friday;
	
	private System.Nullable<byte> _Saturday;
	
	private System.Nullable<byte> _Sunday;
	
	private System.Nullable<int> _Start_date;
	
	private System.Nullable<int> _End_date;
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnService_idChanging(int value);
    partial void OnService_idChanged();
    partial void OnMondayChanging(System.Nullable<byte> value);
    partial void OnMondayChanged();
    partial void OnTuesdayChanging(System.Nullable<byte> value);
    partial void OnTuesdayChanged();
    partial void OnWednesdayChanging(System.Nullable<byte> value);
    partial void OnWednesdayChanged();
    partial void OnThursdayChanging(System.Nullable<byte> value);
    partial void OnThursdayChanged();
    partial void OnFridayChanging(System.Nullable<byte> value);
    partial void OnFridayChanged();
    partial void OnSaturdayChanging(System.Nullable<byte> value);
    partial void OnSaturdayChanged();
    partial void OnSundayChanging(System.Nullable<byte> value);
    partial void OnSundayChanged();
    partial void OnStart_dateChanging(System.Nullable<int> value);
    partial void OnStart_dateChanged();
    partial void OnEnd_dateChanging(System.Nullable<int> value);
    partial void OnEnd_dateChanged();
    #endregion
	
	public Service()
	{
		OnCreated();
	}
	
	[global::System.Data.Linq.Mapping.ColumnAttribute(Name="service_id", Storage="_Service_id", DbType="Int NOT NULL", IsPrimaryKey=true)]
	public int Service_id
	{
		get
		{
			return this._Service_id;
		}
		set
		{
			if ((this._Service_id != value))
			{
				this.OnService_idChanging(value);
				this.SendPropertyChanging();
				this._Service_id = value;
				this.SendPropertyChanged("Service_id");
				this.OnService_idChanged();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.ColumnAttribute(Name="monday", Storage="_Monday", DbType="TinyInt")]
	public System.Nullable<byte> Monday
	{
		get
		{
			return this._Monday;
		}
		set
		{
			if ((this._Monday != value))
			{
				this.OnMondayChanging(value);
				this.SendPropertyChanging();
				this._Monday = value;
				this.SendPropertyChanged("Monday");
				this.OnMondayChanged();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.ColumnAttribute(Name="tuesday", Storage="_Tuesday", DbType="TinyInt")]
	public System.Nullable<byte> Tuesday
	{
		get
		{
			return this._Tuesday;
		}
		set
		{
			if ((this._Tuesday != value))
			{
				this.OnTuesdayChanging(value);
				this.SendPropertyChanging();
				this._Tuesday = value;
				this.SendPropertyChanged("Tuesday");
				this.OnTuesdayChanged();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.ColumnAttribute(Name="wednesday", Storage="_Wednesday", DbType="TinyInt")]
	public System.Nullable<byte> Wednesday
	{
		get
		{
			return this._Wednesday;
		}
		set
		{
			if ((this._Wednesday != value))
			{
				this.OnWednesdayChanging(value);
				this.SendPropertyChanging();
				this._Wednesday = value;
				this.SendPropertyChanged("Wednesday");
				this.OnWednesdayChanged();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.ColumnAttribute(Name="thursday", Storage="_Thursday", DbType="TinyInt")]
	public System.Nullable<byte> Thursday
	{
		get
		{
			return this._Thursday;
		}
		set
		{
			if ((this._Thursday != value))
			{
				this.OnThursdayChanging(value);
				this.SendPropertyChanging();
				this._Thursday = value;
				this.SendPropertyChanged("Thursday");
				this.OnThursdayChanged();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.ColumnAttribute(Name="friday", Storage="_Friday", DbType="TinyInt")]
	public System.Nullable<byte> Friday
	{
		get
		{
			return this._Friday;
		}
		set
		{
			if ((this._Friday != value))
			{
				this.OnFridayChanging(value);
				this.SendPropertyChanging();
				this._Friday = value;
				this.SendPropertyChanged("Friday");
				this.OnFridayChanged();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.ColumnAttribute(Name="saturday", Storage="_Saturday", DbType="TinyInt")]
	public System.Nullable<byte> Saturday
	{
		get
		{
			return this._Saturday;
		}
		set
		{
			if ((this._Saturday != value))
			{
				this.OnSaturdayChanging(value);
				this.SendPropertyChanging();
				this._Saturday = value;
				this.SendPropertyChanged("Saturday");
				this.OnSaturdayChanged();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.ColumnAttribute(Name="sunday", Storage="_Sunday", DbType="TinyInt")]
	public System.Nullable<byte> Sunday
	{
		get
		{
			return this._Sunday;
		}
		set
		{
			if ((this._Sunday != value))
			{
				this.OnSundayChanging(value);
				this.SendPropertyChanging();
				this._Sunday = value;
				this.SendPropertyChanged("Sunday");
				this.OnSundayChanged();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.ColumnAttribute(Name="start_date", Storage="_Start_date", DbType="Int")]
	public System.Nullable<int> Start_date
	{
		get
		{
			return this._Start_date;
		}
		set
		{
			if ((this._Start_date != value))
			{
				this.OnStart_dateChanging(value);
				this.SendPropertyChanging();
				this._Start_date = value;
				this.SendPropertyChanged("Start_date");
				this.OnStart_dateChanged();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.ColumnAttribute(Name="end_date", Storage="_End_date", DbType="Int")]
	public System.Nullable<int> End_date
	{
		get
		{
			return this._End_date;
		}
		set
		{
			if ((this._End_date != value))
			{
				this.OnEnd_dateChanging(value);
				this.SendPropertyChanging();
				this._End_date = value;
				this.SendPropertyChanged("End_date");
				this.OnEnd_dateChanged();
			}
		}
	}
	
	public event PropertyChangingEventHandler PropertyChanging;
	
	public event PropertyChangedEventHandler PropertyChanged;
	
	protected virtual void SendPropertyChanging()
	{
		if ((this.PropertyChanging != null))
		{
			this.PropertyChanging(this, emptyChangingEventArgs);
		}
	}
	
	protected virtual void SendPropertyChanged(String propertyName)
	{
		if ((this.PropertyChanged != null))
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}

[Index(Name="stop_times_index_stop_id", Columns="Stop_id ASC", IsUnique=false)]
[global::System.Data.Linq.Mapping.TableAttribute(Name="stop_times")]
public partial class Stop_time
{
	
	private System.Nullable<int> _Trip_id;
	
	private System.Nullable<int> _Arrival_time;
	
	private System.Nullable<int> _Departure_time;
	
	private System.Nullable<int> _Stop_id;
	
	private System.Nullable<int> _Stop_sequence;
	
	private System.Nullable<int> _Pickup_type;
	
	private System.Nullable<int> _Dropoff_type;
	
	public Stop_time()
	{
	}
	
	[global::System.Data.Linq.Mapping.ColumnAttribute(Name="trip_id", Storage="_Trip_id", DbType="Int")]
	public System.Nullable<int> Trip_id
	{
		get
		{
			return this._Trip_id;
		}
		set
		{
			if ((this._Trip_id != value))
			{
				this._Trip_id = value;
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.ColumnAttribute(Name="arrival_time", Storage="_Arrival_time", DbType="Int")]
	public System.Nullable<int> Arrival_time
	{
		get
		{
			return this._Arrival_time;
		}
		set
		{
			if ((this._Arrival_time != value))
			{
				this._Arrival_time = value;
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.ColumnAttribute(Name="departure_time", Storage="_Departure_time", DbType="Int")]
	public System.Nullable<int> Departure_time
	{
		get
		{
			return this._Departure_time;
		}
		set
		{
			if ((this._Departure_time != value))
			{
				this._Departure_time = value;
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.ColumnAttribute(Name="stop_id", Storage="_Stop_id", DbType="Int")]
	public System.Nullable<int> Stop_id
	{
		get
		{
			return this._Stop_id;
		}
		set
		{
			if ((this._Stop_id != value))
			{
				this._Stop_id = value;
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.ColumnAttribute(Name="stop_sequence", Storage="_Stop_sequence", DbType="Int")]
	public System.Nullable<int> Stop_sequence
	{
		get
		{
			return this._Stop_sequence;
		}
		set
		{
			if ((this._Stop_sequence != value))
			{
				this._Stop_sequence = value;
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.ColumnAttribute(Name="pickup_type", Storage="_Pickup_type", DbType="Int")]
	public System.Nullable<int> Pickup_type
	{
		get
		{
			return this._Pickup_type;
		}
		set
		{
			if ((this._Pickup_type != value))
			{
				this._Pickup_type = value;
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.ColumnAttribute(Name="dropoff_type", Storage="_Dropoff_type", DbType="Int")]
	public System.Nullable<int> Dropoff_type
	{
		get
		{
			return this._Dropoff_type;
		}
		set
		{
			if ((this._Dropoff_type != value))
			{
				this._Dropoff_type = value;
			}
		}
	}
}

[Index(Name="UQ__stops__000000000000009A", Columns="Id ASC", IsUnique=true)]
[global::System.Data.Linq.Mapping.TableAttribute(Name="stops")]
public partial class Stop : INotifyPropertyChanging, INotifyPropertyChanged
{
	
	private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
	
	private int _Id;
	
	private string _Code;
	
	private string _Name;
	
	private string _Description;
	
	private System.Nullable<double> _Latitude;
	
	private System.Nullable<double> _Longitude;
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnCodeChanging(string value);
    partial void OnCodeChanged();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    partial void OnDescriptionChanging(string value);
    partial void OnDescriptionChanged();
    partial void OnLatitudeChanging(System.Nullable<double> value);
    partial void OnLatitudeChanged();
    partial void OnLongitudeChanging(System.Nullable<double> value);
    partial void OnLongitudeChanged();
    #endregion
	
	public Stop()
	{
		OnCreated();
	}
	
	[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Id", DbType="Int NOT NULL", IsPrimaryKey=true)]
	public int Id
	{
		get
		{
			return this._Id;
		}
		set
		{
			if ((this._Id != value))
			{
				this.OnIdChanging(value);
				this.SendPropertyChanging();
				this._Id = value;
				this.SendPropertyChanged("Id");
				this.OnIdChanged();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Code", DbType="NVarChar(64)")]
	public string Code
	{
		get
		{
			return this._Code;
		}
		set
		{
			if ((this._Code != value))
			{
				this.OnCodeChanging(value);
				this.SendPropertyChanging();
				this._Code = value;
				this.SendPropertyChanged("Code");
				this.OnCodeChanged();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Name", DbType="NVarChar(128)")]
	public string Name
	{
		get
		{
			return this._Name;
		}
		set
		{
			if ((this._Name != value))
			{
				this.OnNameChanging(value);
				this.SendPropertyChanging();
				this._Name = value;
				this.SendPropertyChanged("Name");
				this.OnNameChanged();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Description", DbType="NVarChar(512)")]
	public string Description
	{
		get
		{
			return this._Description;
		}
		set
		{
			if ((this._Description != value))
			{
				this.OnDescriptionChanging(value);
				this.SendPropertyChanging();
				this._Description = value;
				this.SendPropertyChanged("Description");
				this.OnDescriptionChanged();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Latitude", DbType="Float")]
	public System.Nullable<double> Latitude
	{
		get
		{
			return this._Latitude;
		}
		set
		{
			if ((this._Latitude != value))
			{
				this.OnLatitudeChanging(value);
				this.SendPropertyChanging();
				this._Latitude = value;
				this.SendPropertyChanged("Latitude");
				this.OnLatitudeChanged();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.ColumnAttribute(Storage="_Longitude", DbType="Float")]
	public System.Nullable<double> Longitude
	{
		get
		{
			return this._Longitude;
		}
		set
		{
			if ((this._Longitude != value))
			{
				this.OnLongitudeChanging(value);
				this.SendPropertyChanging();
				this._Longitude = value;
				this.SendPropertyChanged("Longitude");
				this.OnLongitudeChanged();
			}
		}
	}
	
	public event PropertyChangingEventHandler PropertyChanging;
	
	public event PropertyChangedEventHandler PropertyChanged;
	
	protected virtual void SendPropertyChanging()
	{
		if ((this.PropertyChanging != null))
		{
			this.PropertyChanging(this, emptyChangingEventArgs);
		}
	}
	
	protected virtual void SendPropertyChanged(String propertyName)
	{
		if ((this.PropertyChanged != null))
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}

[Index(Name="UQ__trips__00000000000000C0", Columns="Trip_id ASC", IsUnique=true)]
[global::System.Data.Linq.Mapping.TableAttribute(Name="trips")]
public partial class Trip : INotifyPropertyChanging, INotifyPropertyChanged
{
	
	private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
	
	private int _Trip_id;
	
	private System.Nullable<int> _Route_id;
	
	private System.Nullable<int> _Service_id;
	
	private string _Trip_headsign;
	
	private System.Nullable<int> _Direction;
	
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnTrip_idChanging(int value);
    partial void OnTrip_idChanged();
    partial void OnRoute_idChanging(System.Nullable<int> value);
    partial void OnRoute_idChanged();
    partial void OnService_idChanging(System.Nullable<int> value);
    partial void OnService_idChanged();
    partial void OnTrip_headsignChanging(string value);
    partial void OnTrip_headsignChanged();
    partial void OnDirectionChanging(System.Nullable<int> value);
    partial void OnDirectionChanged();
    #endregion
	
	public Trip()
	{
		OnCreated();
	}
	
	[global::System.Data.Linq.Mapping.ColumnAttribute(Name="trip_id", Storage="_Trip_id", DbType="Int NOT NULL", IsPrimaryKey=true)]
	public int Trip_id
	{
		get
		{
			return this._Trip_id;
		}
		set
		{
			if ((this._Trip_id != value))
			{
				this.OnTrip_idChanging(value);
				this.SendPropertyChanging();
				this._Trip_id = value;
				this.SendPropertyChanged("Trip_id");
				this.OnTrip_idChanged();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.ColumnAttribute(Name="route_id", Storage="_Route_id", DbType="Int")]
	public System.Nullable<int> Route_id
	{
		get
		{
			return this._Route_id;
		}
		set
		{
			if ((this._Route_id != value))
			{
				this.OnRoute_idChanging(value);
				this.SendPropertyChanging();
				this._Route_id = value;
				this.SendPropertyChanged("Route_id");
				this.OnRoute_idChanged();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.ColumnAttribute(Name="service_id", Storage="_Service_id", DbType="Int")]
	public System.Nullable<int> Service_id
	{
		get
		{
			return this._Service_id;
		}
		set
		{
			if ((this._Service_id != value))
			{
				this.OnService_idChanging(value);
				this.SendPropertyChanging();
				this._Service_id = value;
				this.SendPropertyChanged("Service_id");
				this.OnService_idChanged();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.ColumnAttribute(Name="trip_headsign", Storage="_Trip_headsign", DbType="NVarChar(128)")]
	public string Trip_headsign
	{
		get
		{
			return this._Trip_headsign;
		}
		set
		{
			if ((this._Trip_headsign != value))
			{
				this.OnTrip_headsignChanging(value);
				this.SendPropertyChanging();
				this._Trip_headsign = value;
				this.SendPropertyChanged("Trip_headsign");
				this.OnTrip_headsignChanged();
			}
		}
	}
	
	[global::System.Data.Linq.Mapping.ColumnAttribute(Name="direction", Storage="_Direction", DbType="Int")]
	public System.Nullable<int> Direction
	{
		get
		{
			return this._Direction;
		}
		set
		{
			if ((this._Direction != value))
			{
				this.OnDirectionChanging(value);
				this.SendPropertyChanging();
				this._Direction = value;
				this.SendPropertyChanged("Direction");
				this.OnDirectionChanged();
			}
		}
	}
	
	public event PropertyChangingEventHandler PropertyChanging;
	
	public event PropertyChangedEventHandler PropertyChanged;
	
	protected virtual void SendPropertyChanging()
	{
		if ((this.PropertyChanging != null))
		{
			this.PropertyChanging(this, emptyChangingEventArgs);
		}
	}
	
	protected virtual void SendPropertyChanged(String propertyName)
	{
		if ((this.PropertyChanged != null))
		{
			this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}
#pragma warning restore 1591
