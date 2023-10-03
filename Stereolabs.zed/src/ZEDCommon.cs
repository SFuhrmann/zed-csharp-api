//======= Copyright (c) Stereolabs Corporation, All rights reserved. ===============

using System.Runtime.InteropServices;
using System.Numerics;
using System;
using System.Collections.Generic;

/// \defgroup Video_group Video Module
/// \defgroup Depth_group Depth Sensing Module
/// \defgroup Core_group Core Module
/// \defgroup SpatialMapping_group Spatial Mapping Module
/// \defgroup PositionalTracking_group Positional Tracking Module
/// \defgroup Object_group Object Detection Module
/// \defgroup Sensors_group Sensors Module
/// \defgroup Fusion_group Fusion Module

namespace sl
{
    public class ZEDCommon
    {
        public const string NameDLL = "sl_zed_c.dll";
    }

    /// <summary>
    /// Constant for plugin. Should not be changed
    /// </summary>
    public enum Constant
    {
        MAX_OBJECTS = 75,
        /// <summary>
        /// Maximum number of chunks. It's best to get relatively few chunks and to update them quickly.
        /// </summary>
        MAX_SUBMESH = 1000,
        /// <summary>
        /// Max size of trajectory data (number of frames stored)
        /// </summary>
        MAX_BATCH_SIZE = 200,
        /// <summary>
        /// Maximum number of camera can that be instancied at the same time. Used to initialized arrays of cameras (ex: GetDeviceList())
        /// </summary>
        MAX_CAMERA_PLUGIN = 20,
        /// <summary>
        /// Maximum number of camera that can be fused by the Fusion API.
        /// </summary>
        MAX_FUSED_CAMERAS = 20
    };

    ////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////  Core  ////////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region Core Module

    /// \ingroup Core_group
    /// <summary>
    /// Holds a 3x3 matrix that can be marshaled between the
    /// wrapper and C# scripts.
    /// </summary>
    public struct Matrix3x3
    {
        /// <summary>
        /// 3x3 matrix as the float array.
        /// </summary>
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
        public float[] m; //3x3 matrix.

        /// <summary>
        /// Gives the result of the addition between a Matrix3x3 and a specified scalar value.
        /// </summary>
        /// <param name="a">The scalar value</param>
        /// <returns></returns>
        public float3 multiply(float3 a)
        {
            float3 result = new float3();

            result.x = m[0] * a.x + m[1] * a.y + m[2] * a.z;
            result.y = m[3] * a.x + m[4] * a.y + m[5] * a.z;
            result.z = m[6] * a.x + m[7] * a.y + m[8] * a.z;

            return result;
        }
    };

    /// \ingroup Fusion_group
    /// <summary>
    /// Change the type of outputted position (raw data or fusion data projected into zed camera).
    /// </summary>
    public enum POSITION_TYPE
    {
        /// <summary>
        /// The output position will be the raw position data.
        /// </summary>
        RAW = 0,
        /// <summary>
        /// The output position will be the fused position projected into the requested camera repository.
        /// </summary>
        FUSION,
        ///@cond SHOWHIDDEN 
        SL_POSITION_TYPE_LAST
        ///@endcond
    };

    /// <summary>
    /// \ingroup Core_group
    /// Holds a camera resolution as two pointers (for height and width) for easy
    /// passing back and forth to the ZED wrapper.
    /// </summary>
    public struct Resolution
    {
        /// <summary>
        /// Resolution of the image.
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public Resolution(uint width = 0, uint height = 0)
        {
            this.width = (System.UIntPtr)width;
            this.height = (System.UIntPtr)height;
        }
        /// <summary>
        /// width in pixels.
        /// </summary>
        public System.UIntPtr width;
        /// <summary>
        /// height in pixels.
        /// </summary>
        public System.UIntPtr height;
    };

    /// <summary>
    /// \ingroup Core_group
    /// Rect structure to define a rectangle or a ROI in pixels
    /// Use to set ROI target for AEC/AGC
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Rect
    {
        /// <summary>
        /// x coordinates of top-left corner.
        /// </summary>
        public int x;
        /// <summary>
        /// y coordinates of top-left corner.
        /// </summary>
        public int y;
        /// <summary>
        /// width of the rectangle in pixels.
        /// </summary>
        public int width;
        /// <summary>
        /// height of the rectangle in pixels.
        /// </summary>
        public int height;
    };

    ///\ingroup  Core_group
    /// <summary>
    /// List of error codes in the ZED SDK.
    /// </summary><remarks>
    /// Mirrors ERROR_CODE in the ZED C++ SDK. For more info, read:
    /// https://www.stereolabs.com/docs/api/group__Core__group.html#ga4db9ee29f2ff83c71567c12f6bfbf28c
    /// </remarks>
    public enum ERROR_CODE
    {
        /// <summary>
        /// Camera is currently rebooting.
        /// </summary>
        CAMERA_REBOOTING = -1,
        /// <summary>
        /// Operation was successful.
        /// </summary>
        SUCCESS,
        /// <summary>
        /// Standard, generic code for unsuccessful behavior when no other code is more appropriate.
        /// </summary>
        FAILURE,
        /// <summary>
        /// No GPU found, or CUDA capability of the device is not supported.
        /// </summary>
        NO_GPU_COMPATIBLE,
        /// <summary>
        /// Not enough GPU memory for this depth mode. Try a different mode (such as PERFORMANCE).
        /// </summary>
        NOT_ENOUGH_GPUMEM,
        /// <summary>
        /// The ZED camera is not plugged in or detected.
        /// </summary>
        CAMERA_NOT_DETECTED,
        /// <summary>
        /// The MCU that controls the sensors module has an invalid Serial Number. You can try to recover it launching the 'ZED Diagnostic' tool from the command line with the option '-r'.
        /// </summary>
        SENSORS_NOT_INITIALIZED,
        /// <summary>
        /// a ZED Mini or ZED2/2i camera is detected but the inertial sensor cannot be opened. (Never called for original ZED)
        /// </summary>
        SENSOR_NOT_DETECTED,
        /// <summary>
        /// For Nvidia Jetson X1 only - resolution not yet supported (USB3.0 bandwidth).
        /// </summary>
        INVALID_RESOLUTION,
        /// <summary>
        /// USB communication issues. Occurs when the camera FPS cannot be reached, due to a lot of corrupted frames.
        /// Try changing the USB port.
        /// </summary>
        LOW_USB_BANDWIDTH,
        /// <summary>
        /// ZED calibration file is not found on the host machine. Use ZED Explorer or ZED Calibration to get one.
        /// </summary>
        CALIBRATION_FILE_NOT_AVAILABLE,
        /// <summary>
        /// ZED calibration file is not valid. Try downloading the factory one or recalibrating using the ZED Calibration tool.
        /// </summary>
        INVALID_CALIBRATION_FILE,
        /// <summary>
        /// The provided SVO file is not valid.
        /// </summary>
        INVALID_SVO_FILE,
        /// <summary>
        /// An SVO recorder-related error occurred (such as not enough free storage or an invalid file path).
        /// </summary>
        SVO_RECORDING_ERROR,
        /// <summary>
        /// An SVO related error when NVIDIA based compression cannot be loaded
        /// </summary>
        SVO_UNSUPPORTED_COMPRESSION,
        /// <summary>
        /// SVO end of file has been reached, and no frame will be available until the SVO position is reset.
        /// </summary>
        END_OF_SVO_FILE_REACHED,
        /// <summary>
        /// The requested coordinate system is not available.
        /// </summary>
        INVALID_COORDINATE_SYSTEM,
        /// <summary>
        /// The firmware of the ZED is out of date. Update to the latest version.
        /// </summary>
        INVALID_FIRMWARE,
        /// <summary>
        ///  An invalid parameter has been set for the function.
        /// </summary>
        INVALID_FUNCTION_PARAMETERS,
        /// <summary>
        /// In grab() only, a CUDA error has been detected in the process. Activate wrapperVerbose in ZEDManager.cs for more info.
        /// </summary>
        CUDA_ERROR,
        /// <summary>
        /// In grab() only, ZED SDK is not initialized. Probably a missing call to sl::Camera::open.
        /// </summary>
        CAMERA_NOT_INITIALIZED,
        /// <summary>
        /// Your NVIDIA driver is too old and not compatible with your current CUDA version.
        /// </summary>
        NVIDIA_DRIVER_OUT_OF_DATE,
        /// <summary>
        /// The function call is not valid in the current context. Could be a missing a call to sl::Camera::open.
        /// </summary>
        INVALID_FUNCTION_CALL,
        /// <summary>
        ///  The SDK wasn't able to load its dependencies, the installer should be launched.
        /// </summary>
        CORRUPTED_SDK_INSTALLATION,
        /// <summary>
        /// The installed SDK is not the SDK used to compile the program.
        /// </summary>
        INCOMPATIBLE_SDK_VERSION,
        /// <summary>
        /// The given area file does not exist. Check the file path.
        /// </summary>
        INVALID_AREA_FILE,
        /// <summary>
        /// The area file does not contain enough data to be used ,or the sl::DEPTH_MODE used during the creation of the
        /// area file is different from the one currently set.
        /// </summary>
        INCOMPATIBLE_AREA_FILE,
        /// <summary>
        /// Camera failed to set up.
        /// </summary>
        CAMERA_FAILED_TO_SETUP,
        /// <summary>
        /// Your ZED cannot be opened. Try replugging it to another USB port or flipping the USB-C connector (if using ZED Mini).
        /// </summary>
        CAMERA_DETECTION_ISSUE,
        /// <summary>
        /// Cannot start camera stream. Make sure your camera is not already used by another process or blocked by firewall or antivirus.
        /// </summary>
        CAMERA_ALREADY_IN_USE,
        /// <summary>
        /// No GPU found or CUDA is unable to list it. Can be a driver/reboot issue.
        /// </summary>
        NO_GPU_DETECTED,
        /// <summary>
        /// Plane not found. Either no plane is detected in the scene, at the location or corresponding to the floor,
        /// or the floor plane doesn't match the prior given.
        /// </summary>
        PLANE_NOT_FOUND,
        /// <summary>
        /// The Object detection module is only compatible with the ZED 2
        /// </summary>
        MODULE_NOT_COMPATIBLE_WITH_CAMERA,
        /// <summary>
        /// The module needs the sensors to be enabled (see InitParameters::sensors_required)
        /// </summary>
        MOTION_SENSORS_REQUIRED,
        /// <summary>
        /// The module needs a newer version of CUDA
        /// </summary>
        MODULE_NOT_COMPATIBLE_WITH_CUDA_VERSION,
        /// <summary>
        /// End of ERROR_CODE
        /// </summary>
        ERROR_CODE_LAST
    };

    ///\ingroup  Core_group
    /// <summary>
    /// List of available coordinate systems.
    /// </summary>
    public enum COORDINATE_SYSTEM
    {
        /// <summary>
        /// Standard coordinates system used in computer vision.
        /// Used in OpenCV. See: http://docs.opencv.org/2.4/modules/calib3d/doc/camera_calibration_and_3d_reconstruction.html
        /// </summary>
        IMAGE,
        /// <summary>
        /// Left-Handed with Y up and Z forward. Used in Unity3D with DirectX
        /// </summary>
        LEFT_HANDED_Y_UP,
        /// <summary>
        ///  Right-Handed with Y pointing up and Z backward. Used in OpenGL.
        /// </summary>
        RIGHT_HANDED_Y_UP,
        /// <summary>
        /// Right-Handed with Z pointing up and Y forward. Used in 3DSMax.
        /// </summary>
        RIGHT_HANDED_Z_UP,
        /// <summary>
        /// Left-Handed with Z axis pointing up and X forward. Used in Unreal Engine.
        /// </summary>
        LEFT_HANDED_Z_UP,
        /// <summary>
        /// Right-Handed with Z pointing up and X forward. Used in ROS (REP 103)
        /// </summary>
        RIGHT_HANDED_Z_UP_X_FWD
    }

    /// \ingroup Core_group
    /// <summary>
    /// Structure containing information about the camera sensor
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CameraConfiguration
    {
        /// <summary>
        /// Intrinsic and Extrinsic stereo parameters for rectified/undistorded images (default).
        /// </summary>
        public CalibrationParameters calibrationParameters;
        /// <summary>
        /// Intrinsic and Extrinsic stereo parameters for original images (unrectified/distorded).
        /// </summary>
        public CalibrationParameters calibrationParametersRaw;
        /// <summary>
        /// The internal firmware version of the camera.
        /// </summary>
        public uint firmwareVersion;
        /// <summary>
        /// The camera capture FPS
        /// </summary>
        public float fps;
        /// <summary>
        /// The camera resolution
        /// </summary>
        public Resolution resolution;
    };

    /// \ingroup Core_group
    /// <summary>
    /// Structure containing information of a single camera (serial number, model, input type, etc.)
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CameraInformation
    {
        /// <summary>
        /// The serial number of the camera.
        /// </summary>
        public uint serialNumber;
        /// <summary>
        /// The model of the camera (ZED, ZED, ZED2 or ZED2i).
        /// </summary>
        public MODEL cameraModel;
        /// <summary>
        /// Input type used in SDK. 
        /// </summary>
        public INPUT_TYPE inputType;
        /// <summary>
        /// Camera configuration as defined in \ref CameraConfiguration.
        /// </summary>
        public CameraConfiguration cameraConfiguration;
        /// <summary>
        /// Device Sensors configuration as defined in \ref SensorsConfiguration.
        /// </summary>
        public SensorsConfiguration sensorsConfiguration;
    };

    /// \ingroup Video_group
    /// <summary>
    /// Defines the input type used in the ZED SDK. Can be used to select a specific camera with ID or serial number, or a svo file
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct InputType
    {
        /// <summary>
        /// The current input type.
        /// </summary>
        public INPUT_TYPE inputType;
        /// <summary>
        /// Serial Number of the camera
        /// </summary>
	    uint serialNumber;
        /// <summary>
        /// Id of the camera
        /// </summary>
        uint id;
        /// <summary>
        /// path to the SVO file
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        char[] svoInputFilename;
        /// <summary>
        /// Ip address of the streaming camera
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        char[] streamInputIp;
        /// <summary>
        /// port of the streaming camera
        /// </summary>
        ushort streamInputPort;
    };

    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////  Tracking  ////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region Tracking Module

    /// \ingroup PositionalTracking_group
    /// <summary>
    /// Parameters for positional tracking initialization.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class PositionalTrackingParameters
    {
        /// <summary>
        /// Rotation of the camera in the world frame when the camera is started.By default, it should be identity.
        /// </summary>
        public Quaternion initialWorldRotation = Quaternion.Identity;
        /// <summary>
        /// Position of the camera in the world frame when the camera is started. By default, it should be zero.
        /// </summary>
        public Vector3 initialWorldPosition = Vector3.Zero;
        /// <summary>
        /// This mode enables the camera to remember its surroundings. This helps correct positional tracking drift, and can be helpful for positioning different cameras relative to one other in space.
        /// </summary>
        public bool enableAreaMemory = true;
        /// <summary>
        /// This mode enables smooth pose correction for small drift correction.
        /// </summary>
        public bool enablePoseSmothing = false;
        /// <summary>
        /// This mode initializes the tracking to be aligned with the floor plane to better position the camera in space.
        /// </summary>
        public bool setFloorAsOrigin = false;
        /// <summary>
        /// This setting allows you define the camera as static. If true, it will not move in the environment. This allows you to set its position using initial_world_transform.
        /// </summary>
        public bool setAsStatic = false;
        /// <summary>
        /// This setting allows you to enable or disable IMU fusion. When set to false, only the optical odometry will be used.
        /// </summary>
        public bool enableIMUFusion = true;
        /// <summary>
        /// This setting allows you to change the minimum depth used by the SDK for Positional Tracking.
        /// </summary>
        public float depthMinRange = -1f;
        /// <summary>
        /// 
        /// </summary>
        public bool setGravityAsOrigin = true;

        /// <summary>
        /// Area localization file that describes the surroundings, saved from a previous tracking session.
        /// </summary>
        public string areaFilePath = "";
        /// <summary>
        /// Positional tracking mode used. Can be used to improve accuracy in some type of scene at the cost of longer runtime
        /// default : POSITIONAL_TRACKING_MODE::STANDARD
        /// </summary>
        public sl.POSITIONAL_TRACKING_MODE mode = sl.POSITIONAL_TRACKING_MODE.STANDARD;
    }
    /// \ingroup PositionalTracking_group
    /// <summary>
    /// Pose structure with data on timing and validity in addition to
    /// position and rotation.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Pose
    {
        /// <summary>
        /// boolean that indicates if tracking is activated or not. You should check that first if something wrong.
        /// </summary>
        public bool valid;
        /// <summary>
        /// Timestamp of the pose. This timestamp should be compared with the camera timestamp for synchronization.
        /// </summary>
        public ulong timestamp;
        /// <summary>
        /// orientation from the pose.
        /// </summary>
        public Quaternion rotation;
        /// <summary>
        /// translation from the pose.
        /// </summary>
        public Vector3 translation;
        /// <summary>
        /// Confidence/Quality of the pose estimation for the target frame.
        /// A confidence metric of the tracking[0 - 100], 0 means that the tracking is lost, 100 means that the tracking can be fully trusted.
        /// </summary>
        public int pose_confidence;
        /// <summary>
        /// 6x6 Pose covariance of translation (the first 3 values) and rotation in so3 (the last 3 values)
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)]
        public float[] pose_covariance;
        /// <summary>
        /// Twist of the camera available in reference camera, this expresses velocity in free space, broken into its linear and angular parts.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public float[] twist;
        /// <summary>
        /// Row-major representation of the 6x6 twist covariance matrix of the camera, this expresses the uncertainty of the twist.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)]
        public float[] twist_covariance;
    };

    ///\ingroup PositionalTracking_group
    /// <summary>
    /// Possible states of the ZED's Tracking system.
    /// </summary>
    public enum POSITIONAL_TRACKING_STATE
    {
        /// <summary>
        /// Tracking is searching for a match from the database to relocate to a previously known position.
        /// </summary>
        SEARCHING,
        /// <summary>
        /// Tracking is operating normally; tracking data should be correct.
        /// </summary>
        OK,
        /// <summary>
        /// Tracking is not enabled.
        /// </summary>
        OFF,
        /// <summary>
        ///Effective FPS is too low to give proper results for motion tracking. Consider using PERFORMANCES parameters (DEPTH_MODE_PERFORMANCE, low camera resolution (VGA, HD720))
        /// </summary>
        FPS_TOO_LOW,
        /// <summary>
        /// The camera is searching for the floor plane to locate itself related to it, the REFERENCE_FRAME.WORLD will be set afterward.
        /// </summary>
        SEARCHING_FLOOR_PLANE
    }

    ///\ingroup PositionalTracking_group
    /// <summary>
    /// Lists the mode of positional tracking that can be used.
    /// </summary>
    public enum POSITIONAL_TRACKING_MODE
    {
        /// <summary>
        /// Default mode, best compromise in performance and accuracy
        /// </summary>
        STANDARD,
        /// <summary>
        /// Improve accuracy in more challening scenes such as outdoor repetitive patterns like extensive field. 
        /// Curently works best with ULTRA depth mode, requires more compute power
        /// </summary>
        QUALITY
    }


    /// \ingroup PositionalTracking_group
    /// <summary>
    /// Reference frame (world or camera) for tracking and depth sensing.
    /// </summary>
    public enum REFERENCE_FRAME
    {
        /// <summary>
        /// Matrix contains the total displacement from the world origin/the first tracked point.
        /// </summary>
        WORLD,
        /// <summary>
        /// Matrix contains the displacement from the previous camera position to the current one.
        /// </summary>
        CAMERA
    };


    /// \ingroup PositionalTracking_group
    /// <summary>
    /// Part of the ZED (left/right sensor, center) that's considered its center for tracking purposes.
    /// </summary>
    public enum TRACKING_FRAME
    {
        /// <summary>
        /// Camera's center is at the left sensor.
        /// </summary>
		LEFT_EYE,
        /// <summary>
        /// Camera's center is in the camera's physical center, between the sensors.
        /// </summary>
		CENTER_EYE,
        /// <summary>
        /// Camera's center is at the right sensor.
        /// </summary>
		RIGHT_EYE
    };

    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////  Sensors  /////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region Sensors Module

    /// \ingroup Sensors_group
    /// <summary>
    /// Full IMU data structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ImuData
    {
        /// <summary>
        /// Indicates if imu data is available
        /// </summary>
        public bool available;
        /// <summary>
        /// IMU Data timestamp in ns
        /// </summary>
        public ulong timestamp;
        /// <summary>
        /// Gyroscope calibrated data in degrees/second.
        /// </summary>
		public Vector3 angularVelocity;
        /// <summary>
        /// Accelerometer calibrated data in m/s².
        /// </summary>
		public Vector3 linearAcceleration;
        /// <summary>
        /// Gyroscope raw/uncalibrated data in degrees/second.
        /// </summary>
        public Vector3 angularVelocityUncalibrated;
        /// <summary>
        /// Accelerometer raw/uncalibrated data in m/s².
        /// </summary>
		public Vector3 linearAccelerationUncalibrated;
        /// <summary>
        /// Orientation from gyro/accelerator fusion.
        /// </summary>
		public Quaternion fusedOrientation;
        /// <summary>
        /// Covariance matrix of the quaternion.
        /// </summary>
		public Matrix3x3 orientationCovariance;
        /// <summary>
        /// Gyroscope raw data covariance matrix.
        /// </summary>
		public Matrix3x3 angularVelocityCovariance;
        /// <summary>
        /// Accelerometer raw data covariance matrix.
        /// </summary>
		public Matrix3x3 linearAccelerationCovariance;
    };

    /// \ingroup Sensors_group
    [StructLayout(LayoutKind.Sequential)]
    public struct BarometerData
    {
        /// <summary>
        /// Indicates if mag data is available
        /// </summary>
        public bool available;
        /// <summary>
        /// mag Data timestamp in ns
        /// </summary>
        public ulong timestamp;
        /// <summary>
        /// Barometer ambient air pressure in hPa
        /// </summary>
        public float pressure;
        /// <summary>
        /// Relative altitude from first camera position
        /// </summary>
        public float relativeAltitude;
    };

    public enum HEADING_STATE
    {
        /// <summary>
        /// The heading is reliable and not affected by iron interferences.
        /// </summary>
        GOOD,
        /// <summary>
        /// The heading is reliable, but affected by slight iron interferences.
        /// </summary>
        OK,
        /// <summary>
        /// The heading is not reliable because affected by strong iron interferences.
        /// </summary>
        NOT_GOOD,
        /// <summary>
        /// The magnetometer has not been calibrated.
        /// </summary>
        NOT_CALIBRATED,
        /// <summary>
        /// The magnetomer sensor is not available.
        /// </summary>
        MAG_NOT_AVAILABLE,
        /// <summary>
        /// Last heading state.
        /// </summary>
        LAST
    };

    /// \ingroup Sensors_group
    [StructLayout(LayoutKind.Sequential)]
    public struct MagnetometerData
    {
        /// <summary>
        /// Indicates if mag data is available
        /// </summary>
        public bool available;
        /// <summary>
        /// mag Data timestamp in ns
        /// </summary>
        public ulong timestamp;
        /// <summary>
        /// Magnetic field calibrated values in uT
        /// </summary>
        public Vector3 magneticField;
        /// <summary>
        /// Magnetic field raw values in uT
        /// </summary>
        public Vector3 magneticFieldUncalibrated;
        /// <summary>
        /// The camera heading in degrees relative to the magnetic North Pole.
        /// note: The magnetic North Pole has an offset with respect to the geographic North Pole, depending on the
        /// geographic position of the camera.
        /// To get a correct magnetic heading the magnetometer sensor must be calibrated using the ZED Sensor Viewer tool
        /// </summary>
        public float magneticHeading;
        /// <summary>
        /// The state of the /ref magnetic_heading value
        /// </summary>
        public HEADING_STATE magnetic_heading_state;
        /// <summary>
        /// The accuracy of the magnetic heading measure in the range [0.0,1.0].
        /// A negative value means that the magnetometer must be calibrated using the ZED Sensor Viewer tool
        /// </summary>
        public float magnetic_heading_accuracy;
        /// <summary>
        /// Realtime data acquisition rate [Hz]
        /// </summary>
        public float effective_rate;
    };

    /// \ingroup Sensors_group
    [StructLayout(LayoutKind.Sequential)]
    public struct TemperatureSensorData
    {
        /// <summary>
        /// Temperature from IMU device ( -100 if not available)
        /// </summary>
        public float imu_temp;
        /// <summary>
        /// Temperature from Barometer device ( -100 if not available)
        /// </summary>
        public float barometer_temp;
        /// <summary>
        /// Temperature from Onboard left analog temperature sensor ( -100 if not available)
        /// </summary>
        public float onboard_left_temp;
        /// <summary>
        /// Temperature from Onboard right analog temperature sensor ( -100 if not available)
        /// </summary>
        public float onboard_right_temp;
    };

    /// \ingroup Sensors_group
    [StructLayout(LayoutKind.Sequential)]
    public struct SensorsData
    {
        /// <summary>
        /// Contains Imu Data
        /// </summary>
        public ImuData imu;
        /// <summary>
        /// Contains Barometer Data
        /// </summary>
        public BarometerData barometer;
        /// <summary>
        /// Contains Mag Data
        /// </summary>
        public MagnetometerData magnetometer;
        /// <summary>
        /// Contains Temperature Data
        /// </summary>
        public TemperatureSensorData temperatureSensor;
        /// <summary>
        /// Indicated if camera is :
        /// -> Static : 0
        /// -> Moving : 1
        /// -> Falling : 2
        /// </summary>
        public int camera_moving_state;
        /// <summary>
        /// Indicates if the current sensors data is sync to the current image (>=1). Otherwise, value will be 0.
        /// </summary>
        public int image_sync_val;
    };

    /// \ingroup Sensors_group
    /// <summary>
    /// List of the available onboard sensors
    /// </summary>
    public enum SENSOR_TYPE
    {
        /// <summary>
        /// Three axis Accelerometer sensor to measure the inertial accelerations.
        /// </summary>
        ACCELEROMETER,
        /// <summary>
        /// Three axis Gyroscope sensor to measure the angular velocitiers.
        /// </summary>
        GYROSCOPE,
        /// <summary>
        /// Three axis Magnetometer sensor to measure the orientation of the device respect to the earth magnetic field.
        /// </summary>
        MAGNETOMETER,
        /// <summary>
        /// Barometer sensor to measure the atmospheric pressure.
        /// </summary>
        BAROMETER
    };

    /// \ingroup Sensors_group
    /// <summary>
    /// List of the available onboard sensors measurement units
    /// </summary>
    public enum SENSORS_UNIT
    {
        /// <summary>
        /// Acceleration [m/s²].
        /// </summary>
        M_SEC_2,
        /// <summary>
        /// Angular velocity [deg/s].
        /// </summary>
        DEG_SEC,
        /// <summary>
        /// Magnetic Fiels [uT].
        /// </summary>
        U_T,
        /// <summary>
        /// Atmospheric pressure [hPa].
        /// </summary>
        HPA,
        /// <summary>
        /// Temperature [°C].
        /// </summary>
        CELSIUS,
        /// <summary>
        /// Frequency [Hz].
        /// </summary>
        HERTZ
    };

    /// \ingroup Sensors_group
    /// <summary>
    /// Structure containing information about a single sensor available in the current device
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SensorParameters
    {
        /// <summary>
        /// The type of the sensor as \ref DEVICE_SENSORS.
        /// </summary>
        public SENSOR_TYPE type;
        /// <summary>
        /// The resolution of the sensor.
        /// </summary>
        public float resolution;
        /// <summary>
        /// The sampling rate (or ODR) of the sensor.
        /// </summary>
        public float sampling_rate;
        /// <summary>
        /// The range values of the sensor. MIN: `range.x`, MAX: `range.y`
        /// </summary>
        public float2 range;
        /// <summary>
        /// also known as white noise, given as continous (frequency independant). Units will be expressed in sensor_unit/√(Hz). `NAN` if the information is not available.
        /// </summary>
        public float noise_density;
        /// <summary>
        /// derived from the Allan Variance, given as continous (frequency independant). Units will be expressed in sensor_unit/s/√(Hz).`NAN` if the information is not available.
        /// </summary>
        public float random_walk;
        /// <summary>
        /// The string relative to the measurement unit of the sensor.
        /// </summary>
        public SENSORS_UNIT sensor_unit;
        /// <summary>
        ///
        /// </summary>
        public bool isAvailable;
    };

    /// \ingroup Sensors_group
    /// <summary>
    /// Structure containing information about all the sensors available in the current device
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SensorsConfiguration
    {
        /// <summary>
        /// The firmware version of the sensor module, 0 if no sensors are available (ZED camera model).
        /// </summary>
        public uint firmware_version;
        /// <summary>
        /// contains rotation between IMU frame and camera frame.
        /// </summary>
        public float4 camera_imu_rotation;
        /// <summary>
        /// contains translation between IMU frame and camera frame.
        /// </summary>
        public float3 camera_imu_translation;
        /// <summary>
        /// Magnetometer to IMU rotation. contains rotation between IMU frame and magnetometer frame.
        /// </summary>
        public float4 imu_magnometer_rotation;
        /// <summary>
        /// Magnetometer to IMU translation. contains translation between IMU frame and magnetometer frame.
        /// </summary>
        public float3 imu_magnometer_translation;
        /// <summary>
        /// Configuration of the accelerometer device.
        /// </summary>
        public SensorParameters accelerometer_parameters;
        /// <summary>
        /// Configuration of the gyroscope device.
        /// </summary>
        public SensorParameters gyroscope_parameters;
        /// <summary>
        /// Configuration of the magnetometer device.
        /// </summary>
        public SensorParameters magnetometer_parameters;
        /// <summary>
        /// Configuration of the barometer device
        /// </summary>
        public SensorParameters barometer_parameters;
        /// <summary>
        /// if a sensor type is available on the device
        /// </summary>
        /// <param name="sensor_type"></param>
        /// <returns></returns>
        public bool isSensorAvailable(SENSOR_TYPE sensor_type) {
            switch (sensor_type) {
                case SENSOR_TYPE.ACCELEROMETER:
                    return accelerometer_parameters.isAvailable;
                case SENSOR_TYPE.GYROSCOPE:
                    return gyroscope_parameters.isAvailable;
                case SENSOR_TYPE.MAGNETOMETER:
                    return magnetometer_parameters.isAvailable;
                case SENSOR_TYPE.BAROMETER:
                    return barometer_parameters.isAvailable;
                default:
                    break;
            }
            return false;
        }
    };



    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////  Depth Sensing  ///////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region Depth Sensing Module

    ///\ingroup Depth_group
    /// <summary>
    /// Runtime parameters used by the ZEDCamera.Grab() function, and its Camera::grab() counterpart in the SDK.
    /// </summary>
    public class RuntimeParameters
    {
        /// <summary>
        /// Provides 3D measures (point cloud and normals) in the desired reference frame (default is REFERENCE_FRAME_CAMERA).
        /// </summary>
        public sl.REFERENCE_FRAME measure3DReferenceFrame;
        /// <summary>
        /// Defines whether the depth map should be computed.
        /// </summary>
        public bool enableDepth;
        /// <summary>
        /// Defines if the depth map should be completed or not, similar to the removed SENSING_MODE::FILL.
        /// Warning: Enabling this will override the confidence values confidenceThreshold and textureConfidenceThreshold as well as removeSaturatedAreas
        /// </summary>
        public bool enableFillMode;
        /// <summary>
        ///  Defines the confidence threshold for the depth. Based on stereo matching score.
        /// </summary>
        public int confidenceThreshold;
        /// <summary>
        /// Defines texture confidence threshold for the depth. Based on textureness confidence.
        /// </summary>
        public int textureConfidenceThreshold;
        /// <summary>
        /// Defines if the saturated area (Luminance>=255) must be removed from depth map estimation
        /// </summary>
        public bool removeSaturatedAreas;

        /// <summary>
        /// Constructor
        /// </summary>
        public RuntimeParameters(REFERENCE_FRAME reframe = REFERENCE_FRAME.CAMERA, bool depth = true, int cnf_threshold = 100, int txt_cnf_threshold = 100, bool removeSaturatedAreas_ = true, bool pEnableFillMode = false)
        {
            this.measure3DReferenceFrame = reframe;
            this.enableDepth = depth;
            this.confidenceThreshold = cnf_threshold;
            this.textureConfidenceThreshold = txt_cnf_threshold;
            this.removeSaturatedAreas = removeSaturatedAreas_;
            this.enableFillMode = pEnableFillMode;
        }
    }

    ///\ingroup Depth_group
    /// <summary>
    /// Calibration information for an individual sensor on the ZED (left or right). </summary>
    /// <remarks>For more information, see:
    /// https://www.stereolabs.com/docs/api/structsl_1_1CameraParameters.html </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct CameraParameters
    {
        /// <summary>
        /// Focal X.
        /// </summary>
        public float fx;
        /// <summary>
        /// Focal Y.
        /// </summary>
        public float fy;
        /// <summary>
        /// Optical center X.
        /// </summary>
        public float cx;
        /// <summary>
        /// Optical center Y.
        /// </summary>
        public float cy;

        /// <summary>
        /// Distortion coefficients.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U8, SizeConst = 12)]
        public double[] disto;

        /// <summary>
        /// Vertical field of view after stereo rectification.
        /// </summary>
        public float vFOV;
        /// <summary>
        /// Horizontal field of view after stereo rectification.
        /// </summary>
        public float hFOV;
        /// <summary>
        /// Diagonal field of view after stereo rectification.
        /// </summary>
        public float dFOV;
        /// <summary>
        /// Camera's current resolution.
        /// </summary>
        public Resolution resolution;
        /// <summary>
        /// Real focal length in millimeters
        /// </summary>
        public float focalLengthMetric;
    };

    ///\ingroup Depth_group
    /// <summary>
    /// Holds calibration information about the current ZED's hardware, including per-sensor
    /// calibration and offsets between the two sensors.
    /// </summary> <remarks>For more info, see:
    /// https://www.stereolabs.com/docs/api/structsl_1_1CalibrationParameters.html </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct CalibrationParameters
    {
        /// <summary>
        /// Parameters of the left sensor.
        /// </summary>
        public CameraParameters leftCam;
        /// <summary>
        /// Parameters of the right sensor.
        /// </summary>
        public CameraParameters rightCam;
        /// <summary>
        /// Rotation (using Rodrigues' transformation) between the two sensors. Defined as 'tilt', 'convergence' and 'roll'.
        /// </summary>
        public Quaternion Rot;
        /// <summary>
        /// Translation between the two sensors. T[0] is the distance between the two cameras in meters.
        /// </summary>
        public Vector3 Trans;
    };


    ///\ingroup Depth_group
    /// <summary>
    /// Lists available depth computation modes. Each mode offers better accuracy than the
    /// mode before it, but at a performance cost.
    /// </summary><remarks>
    /// Mirrors DEPTH_MODE in the ZED C++ SDK. For more info, see:
    /// https://www.stereolabs.com/docs/api/group__Depth__group.html#ga8d542017c9b012a19a15d46be9b7fa43
    /// </remarks>
    public enum DEPTH_MODE
    {
        /// <summary>
        /// Does not compute any depth map. Only rectified stereo images will be available.
        /// </summary>
        NONE,
        /// <summary>
        /// Fastest mode for depth computation.
        /// </summary>
        PERFORMANCE,
        /// <summary>
        /// Computation mode designed for challenging areas with untextured surfaces.
        /// </summary>
        QUALITY,
        /// <summary>
        /// Native depth. Very accurate, but at a large performance cost.
        /// </summary>
		ULTRA,
        /// <summary>
        ///  End to End Neural disparity estimation, requires AI module
        /// </summary>
        NEURAL
    };

    ///\ingroup Depth_group
    /// <summary>
    /// Lists available measure types retrieved from the camera, used for creating precise measurement maps
    /// (Measure-type textures).
    /// Based on the MEASURE enum in the ZED C++ SDK. For more info, see:
    /// https://www.stereolabs.com/docs/api/group__Depth__group.html#ga798a8eed10c573d759ef7e5a5bcd545d
    /// </summary>
    public enum MEASURE
    {
        /// <summary>
        /// Disparity map. As a ZEDMat, MAT_TYPE is set to MAT_32F_C1.
        /// </summary>
        DISPARITY,
        /// <summary>
        /// Depth map. As a ZEDMat, MAT_TYPE is set to MAT_32F_C1.
        /// </summary>
        DEPTH,
        /// <summary>
        /// Certainty/confidence of the disparity map. As a ZEDMat, MAT_TYPE is set to MAT_32F_C1.
        /// </summary>
        CONFIDENCE,
        /// <summary>
        /// 3D coordinates of the image points. Used for point clouds in ZEDPointCloudManager.
        /// As a ZEDMat, MAT_TYPE is set to MAT_32F_C4. The 4th channel may contain the colors.
        /// </summary>
        XYZ,
        /// <summary>
        /// 3D coordinates and color of the image. As a ZEDMat, MAT_TYPE is set to MAT_32F_C4.
        /// The 4th channel encodes 4 UCHARs for colors in R-G-B-A order.
        /// </summary>
        XYZRGBA,
        /// <summary>
        /// 3D coordinates and color of the image. As a ZEDMat, MAT_TYPE is set to MAT_32F_C4.
        /// The 4th channel encode 4 UCHARs for colors in B-G-R-A order.
        /// </summary>
        XYZBGRA,
        /// <summary>
        /// 3D coordinates and color of the image. As a ZEDMat, MAT_TYPE is set to MAT_32F_C4.
        /// The 4th channel encodes 4 UCHARs for color in A-R-G-B order.
        /// </summary>
        XYZARGB,
        /// <summary>
        /// 3D coordinates and color of the image. As a ZEDMat, MAT_TYPE is set to MAT_32F_C4.
        /// Channel 4 contains color in A-B-G-R order.
        /// </summary>
        XYZABGR,
        /// <summary>
        /// 3D coordinates and color of the image. As a ZEDMat, MAT_TYPE is set to MAT_32F_C4.
        /// The 4th channel encode 4 UCHARs for color in A-B-G-R order.
        /// </summary>
        NORMALS,
        /// <summary>
        /// Disparity map for the right sensor. As a ZEDMat, MAT_TYPE is set to  MAT_32F_C1.
        /// </summary>
        DISPARITY_RIGHT,
        /// <summary>
        /// Depth map for right sensor. As a ZEDMat, MAT_TYPE is set to MAT_32F_C1.
        /// </summary>
        DEPTH_RIGHT,
        /// <summary>
        /// Point cloud for right sensor. As a ZEDMat, MAT_TYPE is set to MAT_32F_C4. Channel 4 is empty.
        /// </summary>
        XYZ_RIGHT,
        /// <summary>
        /// Colored point cloud for right sensor. As a ZEDMat, MAT_TYPE is set to MAT_32F_C4.
        /// Channel 4 contains colors in R-G-B-A order.
        /// </summary>
        XYZRGBA_RIGHT,
        /// <summary>
        /// Colored point cloud for right sensor. As a ZEDMat, MAT_TYPE is set to MAT_32F_C4.
        /// Channel 4 contains colors in B-G-R-A order.
        /// </summary>
        XYZBGRA_RIGHT,
        /// <summary>
        ///  Colored point cloud for right sensor. As a ZEDMat, MAT_TYPE is set to MAT_32F_C4.
        ///  Channel 4 contains colors in A-R-G-B order.
        /// </summary>
        XYZARGB_RIGHT,
        /// <summary>
        /// Colored point cloud for right sensor. As a ZEDMat, MAT_TYPE is set to MAT_32F_C4.
        /// Channel 4 contains colors in A-B-G-R order.
        /// </summary>
        XYZABGR_RIGHT,
        /// <summary>
        ///  Normals vector for right view. As a ZEDMat, MAT_TYPE is set to MAT_32F_C4.
        ///  Channel 4 is empty (set to 0).
        /// </summary>
        NORMALS_RIGHT,
        /// <summary>
        /// Depth map in millimeter. Each pixel  contains 1 unsigned short. As a Mat, MAT_TYPE is set to MAT_U16_C1.
        /// </summary>
        DEPTH_U16_MM,
        /// <summary>
        /// Depth map in millimeter for right sensor. Each pixel  contains 1 unsigned short. As a Mat, MAT_TYPE is set to MAT_U16_C1.
        /// </summary>
        DEPTH_U16_MM_RIGHT
    };

    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////  Video   //////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region Video Module

    ///\ingroup Video_group
    /// <summary>
    /// Struct containing all parameters passed to the SDK when initializing the ZED.
    /// These parameters will be fixed for the whole execution life time of the camera.
    /// <remarks>For more details, see the InitParameters class in the SDK API documentation:
    /// https://www.stereolabs.com/docs/api/structsl_1_1InitParameters.html
    /// </remarks>
    /// </summary>

    public class InitParameters
    {
        public sl.INPUT_TYPE inputType;
        /// <summary>
        /// Resolution the ZED will be set to.
        /// </summary>
        public sl.RESOLUTION resolution;
        /// <summary>
        /// Requested FPS for this resolution. Setting it to 0 will choose the default FPS for this resolution.
        /// </summary>
        public int cameraFPS;
        /// <summary>
        /// ID for identifying which of multiple connected ZEDs to use.
        /// </summary>
        public int cameraDeviceID;
        /// <summary>
        /// Path to a recorded SVO file to play, including filename.
        /// </summary>
        public string pathSVO = "";
        /// <summary>
        /// In SVO playback, this mode simulates a live camera and consequently skipped frames if the computation framerate is too slow.
        /// </summary>
        public bool svoRealTimeMode;
        /// <summary>
        ///  Define a unit for all metric values (depth, point clouds, tracking, meshes, etc.).
        /// </summary>
        public UNIT coordinateUnits;
        /// <summary>
        /// This defines the order and the direction of the axis of the coordinate system.
        /// </summary>
        public COORDINATE_SYSTEM coordinateSystem;
        /// <summary>
        /// Quality level of depth calculations. Higher settings improve accuracy but cost performance.
        /// </summary>
        public sl.DEPTH_MODE depthMode;
        /// <summary>
        /// Minimum distance from the camera from which depth will be computed, in the defined coordinateUnit.
        /// </summary>
        public float depthMinimumDistance;
        /// <summary>
        ///   When estimating the depth, the SDK uses this upper limit to turn higher values into \ref TOO_FAR ones.
        ///   The current maximum distance that can be computed in the defined \ref UNIT.
        ///   Changing this value has no impact on performance and doesn't affect the positional tracking nor the spatial mapping. (Only the depth, point cloud, normals)
        /// </summary>
        public float depthMaximumDistance;
        /// <summary>
        ///  Defines if images are horizontally flipped.
        /// </summary>
        public FLIP_MODE cameraImageFlip;
        /// <summary>
        /// Defines if measures relative to the right sensor should be computed (needed for MEASURE_XXX_RIGHT). 
        /// </summary>
        public bool enableRightSideMeasure;
        /// <summary>
        /// True to disable self-calibration and use the optional calibration parameters without optimizing them.
        /// False is recommended, so that calibration parameters can be optimized.
        /// </summary>
        public bool cameraDisableSelfCalib;
        /// <summary>
        /// True for the SDK to provide text feedback.
        /// </summary>
        public int sdkVerbose;
        /// <summary>
        /// ID of the graphics card on which the ZED's computations will be performed.
        /// </summary>
        public int sdkGPUId;
        /// <summary>
        /// If set to verbose, the filename of the log file into which the SDK will store its text output.
        /// </summary>
        public string sdkVerboseLogFile = "";
        /// <summary>
        /// True to stabilize the depth map. Recommended.
        /// </summary>
        public int depthStabilization;
        /// <summary>
        /// Optional path for searching configuration (calibration) file SNxxxx.conf. (introduced in ZED SDK 2.6)
        /// </summary>
        public string optionalSettingsPath = "";
        /// <summary>
        /// True to stabilize the depth map. Recommended.
        /// </summary>
        public bool sensorsRequired;
        /// <summary>
        /// Path to a recorded SVO file to play, including filename.
        /// </summary>
        public string ipStream = "";
        /// <summary>
        /// Path to a recorded SVO file to play, including filename.
        /// </summary>
        public ushort portStream = 30000;
        /// <summary>
        /// Whether to enable improved color/gamma curves added in ZED SDK 3.0.
        /// </summary>
        public bool enableImageEnhancement = true;
        /// <summary>
        /// Set an optional file path where the SDK can find a file containing the calibration information of the camera computed by OpenCV.
        /// <remarks> Using this will disable the factory calibration of the camera. </remarks>
        /// <warning> Erroneous calibration values can lead to poor SDK modules accuracy. </warning>
        /// </summary>
        public string optionalOpencvCalibrationFile;
        /// <summary>
        /// Define a timeout in seconds after which an error is reported if the \ref open() command fails.
        /// Set to '-1' to try to open the camera endlessly without returning error in case of failure.
        /// Set to '0' to return error in case of failure at the first attempt.
        /// This parameter only impacts the LIVE mode.
        /// </summary>
        public float openTimeoutSec;

        /// <summary>
        /// Define the behavior of the automatic camera recovery during grab() function call. When async is enabled and there's an issue with the communication with the camera
        /// the grab() will exit after a short period and return the ERROR_CODE::CAMERA_REBOOTING warning.The recovery will run in the background until the correct communication is restored.
        /// When async_grab_camera_recovery is false, the grab() function is blocking and will return only once the camera communication is restored or the timeout is reached.
        /// The default behavior is synchronous (false), like previous ZED SDK versions
        /// </summary>
        public bool asyncGrabCameraRecovery;
        /// <summary>
        /// Define a computation upper limit to the grab frequency.
        /// This can be useful to get a known constant fixed rate or limit the computation load while keeping a short exposure time by setting a high camera capture framerate.

        /// The value should be inferior to the InitParameters::camera_fps and strictly positive.It has no effect when reading an SVO file.
        /// This is an upper limit and won't make a difference if the computation is slower than the desired compute capping fps.
        /// \note Internally the grab function always tries to get the latest available image while respecting the desired fps as much as possible.
        /// </summary>
        public float grabComputeCappingFPS = 0;


        /// <summary>
        /// Constructor. Sets default initialization parameters.
        /// </summary>
        public InitParameters()
        {
            this.inputType = sl.INPUT_TYPE.USB;
            this.resolution = RESOLUTION.HD720;
            this.cameraFPS = 60;
            this.cameraDeviceID = 0;
            this.pathSVO = "";
            this.svoRealTimeMode = false;
            this.coordinateUnits = UNIT.METER;
            this.coordinateSystem = COORDINATE_SYSTEM.IMAGE;
            this.depthMode = DEPTH_MODE.PERFORMANCE;
            this.depthMinimumDistance = -1;
            this.depthMaximumDistance = -1;
            this.cameraImageFlip = FLIP_MODE.AUTO;
            this.cameraDisableSelfCalib = false;
            this.sdkVerbose = 0;
            this.sdkGPUId = -1;
            this.sdkVerboseLogFile = "";
            this.enableRightSideMeasure = false;
            this.depthStabilization = 1;
            this.optionalSettingsPath = "";
            this.sensorsRequired = false;
            this.ipStream = "";
            this.portStream = 30000;
            this.enableImageEnhancement = true;
            this.optionalOpencvCalibrationFile = "";
            this.openTimeoutSec = 5.0f;
            this.asyncGrabCameraRecovery = false;
            this.grabComputeCappingFPS = 0;
        }

    }

    ///\ingroup  Video_group
    /// <summary>
    /// Lists available input type in SDK.
    /// </summary>
    public enum INPUT_TYPE
    {
        /// <summary>
        /// USB input mode
        /// </summary>
        USB,
        /// <summary>
        /// SVO file input mode
        /// </summary>
        SVO,
        /// <summary>
        /// STREAM input mode (requires to use enableStreaming()/disableStreaming() on the "sender" side)
        /// </summary>
        STREAM,
        /// <summary>
        /// GMSL input mode
        /// </summary>
        GMSL
    };

    ///\ingroup  Video_group
    /// <summary>
    /// Lists available input type in SDK
    /// </summary>
    public enum BUS_TYPE
    {
        /// <summary>
        /// USB input mode
        /// </summary>
        USB,
        /// <summary>
        /// GMSL input mode (only on NVIDIA Jetson)
        /// </summary>
        GMSL,
        /// <summary>
        /// Automatically select the input type (trying first for availabled USB cameras, then GMSL)
        /// </summary>
        AUTO,
        /// <summary>
        /// Last input mode
        /// </summary>
        LAST
    };



    ///\ingroup  Video_group
    /// <summary>
    /// List of possible camera state
    /// </summary>
    public enum CAMERA_STATE
    {
        /// <summary>
        /// Defines if the camera can be opened by the SDK 
        /// </summary>
        AVAILABLE,
        /// <summary>
        /// Defines if the camera is already opened and unavailable
        /// </summary>
        NOT_AVAILABLE
    };

    ///\ingroup  Video_group
    /// <summary>
    /// Sets the recording parameters.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct RecordingParameters
    {
        /// <summary>
        /// filename of the SVO file.
        /// </summary>
        public string videoFilename;
        /// <summary>
        /// compression_mode : can be one of the SVO_COMPRESSION_MODE enum
        /// </summary>
        public SVO_COMPRESSION_MODE compressionMode;
        /// <summary>
        /// defines the target framerate for the recording module.
        /// </summary>
        public int targetFPS;
        /// <summary>
        /// bitrate : override default bitrate of the SVO file, in KBits/s. Only works if SVO_COMPRESSION_MODE is H264 or H265. : 0 means default values (depends on the resolution)
        /// </summary>
        public uint bitrate;
        /// <summary>
        /// In case of streaming input, if set to false, it will avoid decoding/re-encoding and convert directly streaming input into a SVO file.
        /// This saves a encoding session and can be especially useful on NVIDIA Geforce cards where the number of encoding session is limited.
        /// </summary>
        public bool transcode;
        /// <summary>
        /// Constructor
        /// </summary>
        public RecordingParameters(string filename = "", SVO_COMPRESSION_MODE compression = SVO_COMPRESSION_MODE.H264_BASED, uint bitrate = 0, int fps = 0, bool transcode = false)
        {
            this.videoFilename = filename;
            this.compressionMode = compression;
            this.bitrate = bitrate;
            this.targetFPS = fps;
            this.transcode = transcode;
        }
    }

    ///\ingroup  Video_group
    /// <summary>
    /// Sets the streaming parameters.
    /// </summary>
    public struct StreamingParameters
    {
        /// <summary>
        /// Defines the codec used for streaming.
        /// </summary>
        public STREAMING_CODEC codec;
        /// <summary>
        /// Defines the port used for streaming.
        /// </summary>
        public ushort port;
        /// <summary>
        /// Defines the streaming bitrate in Kbits/s.
        /// </summary>
        public uint bitrate;
        /// <summary>
        /// Defines the gop size in number of frames.
        /// </summary>
        public int gopSize;
        /// <summary>
        /// Enable/Disable adaptive bitrate.
        /// </summary>
        public bool adaptativeBitrate;
        /// <summary>
        /// Defines a single chunk size.
        /// </summary>
        public ushort chunkSize;
        /// <summary>
        /// defines the target framerate for the streaming output.
        /// </summary>
        public int targetFPS;
        /// <summary>
        /// Constructor
        /// </summary>
        public StreamingParameters(STREAMING_CODEC codec = STREAMING_CODEC.H264_BASED, ushort port = 3000, uint bitrate = 8000,
                                    int gopSize = -1, bool adaptativeBitrate = false, ushort chunkSize = 32789, int targetFPS = 0)
        {
            this.codec = codec;
            this.port = port;
            this.bitrate = bitrate;
            this.gopSize = gopSize;
            this.adaptativeBitrate = adaptativeBitrate;
            this.chunkSize = chunkSize;
            this.targetFPS = targetFPS;
        }
    }

    ///\ingroup  Video_group
    /// <summary>
    /// Device properties
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct DeviceProperties
    {
        /// <summary>
        /// The camera state
        /// </summary>
        public sl.CAMERA_STATE cameraState;
        /// <summary>
        /// The camera id (Notice that only the camera with id '0' can be used on Windows)
        /// </summary>
        public int id;
        /// <summary>
        /// The camera system path
        /// </summary>
        public string path;
        /// <summary>
        /// The camera model
        /// </summary>
        public sl.MODEL cameraModel;
        /// <summary>
        /// The camera serial number
        /// </summary>
        public uint sn;
        /// <summary>
        /// The camera input type. Support for INPUT_TYPE.GMSL is only available on hosts with Nvidia Jetson SoC.
        /// </summary>
        public sl.INPUT_TYPE inputType;

    };

    ///\ingroup  Video_group
    /// <summary>
    /// Streaming device properties
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct StreamingProperties
    {
        /// <summary>
        /// The streaming IP of the device
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string ip;
        /// <summary>
        /// The streaming port
        /// </summary>
        public ushort port;
        /// <summary>
        /// The current bitrate of encoding of the streaming device
        /// </summary>
        public int currentBitrate;
        /// <summary>
        /// The current codec used for compression in streaming device
        /// </summary>
        public sl.STREAMING_CODEC codec;
    };

    ///\ingroup  Video_group
    /// <summary>
    /// Container for information about the current SVO recording process.
    /// </summary><remarks>
    /// Mirrors RecordingStatus in the ZED C++ SDK. For more info, visit:
    /// https://www.stereolabs.com/docs/api/structsl_1_1RecordingStatus.html
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    public struct RecordingStatus
    {
        /// <summary>
        /// Recorder status, true if enabled.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool is_recording;
        /// <summary>
        /// Recorder status, true if the pause is enabled.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool is_paused;
        /// <summary>
        /// Status of the current frame. True if recording was successful, false if frame could not be written.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool status;
        /// <summary>
        /// Compression time for the current frame in milliseconds.
        /// </summary>
        public double current_compression_time;
        /// <summary>
        /// Compression ratio (% of raw size) for the current frame.
        /// </summary>
        public double current_compression_ratio;
        /// <summary>
        /// Average compression time in millisecond since beginning of recording.
        /// </summary>
        public double average_compression_time;
        /// <summary>
        /// Compression ratio (% of raw size) since recording was started.
        /// </summary>
        public double average_compression_ratio;
    }

    ///\ingroup  Video_group
    /// <summary>
    /// Represents the available resolution options.\n
    /// Warning: All resolution are not available for every camera. You can find the available resolutions for each camera in <a href="https://www.stereolabs.com/docs/video/camera-controls#selecting-a-resolution">our documentation</a>.
    /// </summary>
    public enum RESOLUTION
    {
        /// <summary>
        /// 2208*1242. Supported frame rate: 15 FPS.
        /// </summary>
        HD2K,
        /// <summary>
        /// 1920*1080. Supported frame rates: 15, 30 FPS.
        /// </summary>
        HD1080,
        /// <summary>
        /// 1920*1200. Supported frame rates: 15, 30, 60 fps.
        /// </summary>
        HD1200,
        /// <summary>
        /// 1280*720. Supported frame rates: 15, 30, 60 FPS.
        /// </summary>
        HD720,
        /// <summary>
        /// 960*600. Supported frame rates: 15, 30, 60, 120 fps.
        /// </summary>
        HDSVGA,
        /// <summary>
        /// 672*376. Supported frame rates: 15, 30, 60, 100 FPS.
        /// </summary>
        VGA,
        /// <summary>
        /// Select the resolution compatible with camera, on ZED X HD1200, HD720 otherwise.
        /// </summary>
        AUTO
    };

    ///\ingroup  Video_group
    /// <summary>
    ///brief Lists available compression modes for SVO recording.
    /// </summary>
    public enum FLIP_MODE
    {
        /// <summary>
        /// default behavior.
        /// </summary>
        OFF = 0,
        /// <summary>
        /// Images and camera sensors data are flipped, useful when your camera is mounted upside down.
        /// </summary>
        ON = 1,
        /// <summary>
        /// in live mode: use the camera orientation (if an IMU is available) to set the flip mode, in SVO mode, read the state of this enum when recorded
        /// </summary>
        AUTO = 2,
    };

    ///\ingroup  Video_group
    /// <summary>
    /// Types of compatible ZED cameras. SL_MODEL in C wrapper.
    /// </summary>
    public enum MODEL
    {
        /// <summary>
        /// ZED(1)
        /// </summary>
	    ZED,
        /// <summary>
        /// ZED Mini.
        /// </summary>
	    ZED_M,
        /// <summary>
        /// ZED2.
        /// </summary>
        ZED2,
        /// <summary>
        /// ZED2i
        /// </summary>
        ZED2i,
        /// <summary>
        /// ZED X
        /// </summary>
        ZED_X,
        /// <summary>
        /// ZED X Mini
        /// </summary>
        ZED_XM
    };

    ///\ingroup  Video_group
    /// <summary>
    /// Lists available view types retrieved from the camera, used for creating human-viewable (Image-type) textures.
    /// </summary><remarks>
    /// Based on the VIEW enum in the ZED C++ SDK. For more info, see:
    /// https://www.stereolabs.com/docs/api/group__Video__group.html#ga77fc7bfc159040a1e2ffb074a8ad248c
    /// </remarks>
    public enum VIEW
    {
        /// <summary>
        /// Left RGBA image. As a Mat, MAT_TYPE is set to MAT_TYPE_8U_C4.
        /// </summary>
        LEFT,
        /// <summary>
        /// Right RGBA image. As a Mat, MAT_TYPE is set to sl::MAT_TYPE_8U_C4.
        /// </summary>
        RIGHT,
        /// <summary>
        /// Left GRAY image. As a Mat, MAT_TYPE is set to sl::MAT_TYPE_8U_C1.
        /// </summary>
        LEFT_GREY,
        /// <summary>
        /// Right GRAY image. As a Mat, MAT_TYPE is set to sl::MAT_TYPE_8U_C1.
        /// </summary>
        RIGHT_GREY,
        /// <summary>
        /// Left RGBA unrectified image. As a Mat, MAT_TYPE is set to sl::MAT_TYPE_8U_C4.
        /// </summary>
        LEFT_UNRECTIFIED,
        /// <summary>
        /// Right RGBA unrectified image. As a Mat, MAT_TYPE is set to sl::MAT_TYPE_8U_C4.
        /// </summary>
        RIGHT_UNRECTIFIED,
        /// <summary>
        /// Left GRAY unrectified image. As a ZEDMat, MAT_TYPE is set to sl::MAT_TYPE_8U_C1.
        /// </summary>
        LEFT_UNRECTIFIED_GREY,
        /// <summary>
        /// Right GRAY unrectified image. As a Mat, MAT_TYPE is set to sl::MAT_TYPE_8U_C1.
        /// </summary>
        RIGHT_UNRECTIFIED_GREY,
        /// <summary>
        ///  Left and right image. Will be double the width to hold both. As a Mat, MAT_TYPE is set to MAT_8U_C4.
        /// </summary>
        SIDE_BY_SIDE,
        /// <summary>
        /// Normalized depth image. As a Mat, MAT_TYPE is set to sl::MAT_TYPE_8U_C4.
        /// <para>Use an Image texture for viewing only. For measurements, use a Measure type instead
        /// (ZEDCamera.RetrieveMeasure()) to preserve accuracy. </para>
        /// </summary>
        DEPTH,
        /// <summary>
        /// Normalized confidence image. As a Mat, MAT_TYPE is set to MAT_8U_C4.
        /// <para>Use an Image texture for viewing only. For measurements, use a Measure type instead
        /// (ZEDCamera.RetrieveMeasure()) to preserve accuracy. </para>
        /// </summary>
        CONFIDENCE,
        /// <summary>
        /// Color rendering of the normals. As a Mat, MAT_TYPE is set to MAT_8U_C4.
        /// <para>Use an Image texture for viewing only. For measurements, use a Measure type instead
        /// (ZEDCamera.RetrieveMeasure()) to preserve accuracy. </para>
        /// </summary>
        NORMALS,
        /// <summary>
        /// Color rendering of the right depth mapped on right sensor. As a Mat, MAT_TYPE is set to MAT_8U_C4.
        /// <para>Use an Image texture for viewing only. For measurements, use a Measure type instead
        /// (ZEDCamera.RetrieveMeasure()) to preserve accuracy. </para>
        /// </summary>
        DEPTH_RIGHT,
        /// <summary>
        /// Color rendering of the normals mapped on right sensor. As a Mat, MAT_TYPE is set to MAT_8U_C4.
        /// <para>Use an Image texture for viewing only. For measurements, use a Measure type instead
        /// (ZEDCamera.RetrieveMeasure()) to preserve accuracy. </para>
        /// </summary>
        NORMALS_RIGHT,
    };

    ///\ingroup  Video_group
    /// <summary>
    ///  Lists available camera settings for the ZED camera (contrast, hue, saturation, gain, etc.)
    /// </summary>
    public enum VIDEO_SETTINGS
    {
        /// <summary>
        /// Brightness control. Value should be between 0 and 8.
        /// </summary>
        BRIGHTNESS,
        /// <summary>
        /// Contrast control. Value should be between 0 and 8.
        /// </summary>
        CONTRAST,
        /// <summary>
        /// Hue control. Value should be between 0 and 11.
        /// </summary>
        HUE,
        /// <summary>
        /// Saturation control. Value should be between 0 and 8.
        /// </summary>
        SATURATION,
        /// <summary>
        /// Sharpness control. Value should be between 0 and 8.
        /// </summary>
        SHARPNESS,
        /// <summary>
        /// Gamma control. Value should be between 1 and 9
        /// </summary>
        GAMMA,
        /// <summary>
        /// Gain control. Value should be between 0 and 100 for manual control.
        /// If ZED_EXPOSURE is set to -1 (automatic mode), then gain will be automatic as well.
        /// </summary>
        GAIN,
        /// <summary>
        /// Exposure control. Value can be between 0 and 100.
        /// Setting to -1 enables auto exposure and auto gain.
        /// Setting to 0 disables auto exposure but doesn't change the last applied automatic values.
        /// Setting to 1-100 disables auto mode and sets exposure to the chosen value.
        /// </summary>
        EXPOSURE,
        /// <summary>
        /// Auto-exposure and auto gain. Setting this to true switches on both. Assigning a specifc value to GAIN or EXPOSURE will set this to 0.
        /// </summary>
        AEC_AGC,
        /// <summary>
        /// ROI for auto exposure/gain. ROI defines the target where the AEC/AGC will be calculated
        /// Use overloaded function for this enum
        /// </summary>
        AEC_AGC_ROI,
        /// <summary>
        /// Color temperature control. Value should be between 2800 and 6500 with a step of 100.
        /// </summary>
        WHITEBALANCE_TEMPERATURE,
        /// <summary>
        /// Defines if the white balance is in automatic mode or not.
        /// </summary>
        WHITEBALANCE_AUTO,
        /// <summary>
        /// front LED status (1==enable, 0 == disable)
        /// </summary>
        LED_STATUS,
        EXPOSURE_TIME,
        ANALOG_GAIN,
        DIGITAL_GAIN,
        AUTO_EXPOSURE_TIME_RANGE,
        /// <summary>
        /// Defines the range of sensor gain in automatic control. Used in setCameraSettings(VIDEO_SETTINGS,int,int). Min/Max range between [1000 - 16000]mdB
        /// </summary>
        AUTO_ANALOG_GAIN_RANGE,
        /// <summary>
        /// Defines the range of digital ISP gain in automatic control. Used in setCameraSettings(VIDEO_SETTINGS,int,int)
        /// </summary>
        AUTO_DIGITAL_GAIN_RANGE,
        /// <summary>
        /// Exposure target compensation made after AE. Reduces the overall illumination by factor of F-stops. values range is [0 - 100] (mapped between [-2.0,2.0]).  Only available for GMSL based cameras.
        /// </summary>
        EXPOSURE_COMPENSATION,
        /// <summary>
        /// Defines the level of denoising applied on both left and right images. values range is [0-100]. Only available for GMSL based cameras.
        /// </summary>
        DENOISING,
        LAST
    };

    ///\ingroup  Video_group
    /// <summary>
    /// Categories indicating when a timestamp is captured.
    /// </summary>
    public enum TIME_REFERENCE
    {
        /// <summary>
        /// Timestamp from when the image was received over USB from the camera, defined
        /// by when the entire image was available in memory.
        /// </summary>
        IMAGE,
        /// <summary>
        /// Timestamp from when the relevant function was called.
        /// </summary>
        CURRENT
    };

    ///\ingroup  Video_group
    /// <summary>
    /// SVO compression modes.
    /// </summary>
    public enum SVO_COMPRESSION_MODE
    {
        /// <summary>
        /// Lossless compression based on png/zstd. Average size = 42% of RAW.
        /// </summary>
        LOSSLESS_BASED,
        /// <summary>
        /// H264(AVCHD) GPU based compression : avg size = 1% (of RAW). Requires a NVIDIA GPU
        /// </summary>
        H264_BASED,
        /// <summary>
        /// H265(HEVC) GPU based compression: avg size = 1% (of RAW). Requires a NVIDIA GPU, Pascal architecture or newer
        /// </summary>
        H265_BASED,
        /// <summary>
        /// H264 Lossless GPU/Hardware based compression: avg size = 25% (of RAW). Provides a SSIM/PSNR result (vs RAW) >= 99.9%. Requires a NVIDIA GPU
        /// </summary>
        H264_LOSSLESS_BASED,
        /// <summary>
        /// H265 Lossless GPU/Hardware based compression: avg size = 25% (of RAW). Provides a SSIM/PSNR result (vs RAW) >= 99.9%. Requires a NVIDIA GPU
        /// </summary>
        H265_LOSSLESS_BASED,
    }

    ///\ingroup  Video_group
    /// <summary>
    /// Streaming codecs
    /// </summary>
    public enum STREAMING_CODEC
    {
        /// <summary>
        /// AVCHD/H264 Based compression
        /// </summary>
        H264_BASED,
        /// <summary>
        /// HEVC/H265 Based compression
        /// </summary>
        H265_BASED
    }
    /// <summary>
    /// defines left,right,both to distinguish between left and right or both sides
    /// </summary>
    public enum SIDE
    {
        /// <summary>
        /// Left side only.
        /// </summary>
        LEFT = 0,
        /// <summary>
        /// Right side only.
        /// </summary>
        RIGHT = 1,
        /// <summary>
        /// Left and Right side.
        /// </summary>
        BOTH = 2
    }
    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////  Spatial Mapping //////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region Spatial Mapping Module

    ///\ingroup SpatialMapping_group
    /// <summary>
    /// Sets the plane detection parameters.
    /// </summary>
    public class PlaneDetectionParameters
    {
        /// <summary>
        /// Controls the spread of plane by checking the position difference.
        /// Default is 0.15 meters.
        /// </summary>
        public float maxDistanceThreshold = 0.15f;

        /// <summary>
        /// Controls the spread of plane by checking the angle difference.
        /// Default is 15 degrees.
        /// </summary>
        public float normalSimilarityThreshold = 15.0f;
    }

    ///\ingroup SpatialMapping_group
    /// <summary>
    /// Sets the spatial mapping parameters.
    /// </summary>
    public class SpatialMappingParameters
    {
        /// <summary>
        /// Spatial mapping resolution in meters.
        /// </summary>
        public float resolutionMeter = 0.0f;
        /// <summary>
        /// Depth range in meters.
        /// </summary>
        public float rangeMeter = 0.05f;
        /// <summary>
        /// Set to true if you want to be able to apply the texture to your mesh after its creation.
        /// </summary>
        public bool saveTexture = false;
        /// <summary>
        /// Set to false if you want to ensure consistency between the mesh and its inner chunk data (default is false).
        /// </summary>
        public bool useChunkOnly = false;
        /// <summary>
        /// 
        /// </summary>
        public int maxMemoryUsage = 2048;
        /// <summary>
        /// Specify if the order of the vertices of the triangles needs to be inverted. If your display process does not handle front and back face culling, you can use this to correct it.
        /// </summary>
        public bool reverseVertexOrder = false;
        /// <summary>
        /// The type of spatial map to be created. This dictates the format that will be used for the mapping(e.g. mesh, point cloud). See \ref SPATIAL_MAP_TYPE
        /// </summary>
        public SPATIAL_MAP_TYPE map_type = SPATIAL_MAP_TYPE.MESH;
        /// <summary>
        ///  Control the integration rate of the current depth into the mapping process.
        /// This parameter controls how many times a stable 3D points should be seen before it is integrated into the spatial mapping.
        /// Default value is 0, this will define the stability counter based on the mesh resolution, the higher the resolution, the higher the stability counter.
        /// </summary>
        public int stabilityCounter = 0;
        /// <summary>
        /// Constructor
        /// </summary>
        public SpatialMappingParameters(float resolutionMeter = 0.05f, float rangeMeter = 0.0f, bool saveTexture = false, SPATIAL_MAP_TYPE map_type = SPATIAL_MAP_TYPE.MESH,
                                        bool useChunkOnly = false, int maxMemoryUsage = 2048, bool reverseVertexOrder = false, int stabilityCounter = 0)
        {
            this.resolutionMeter = resolutionMeter;
            this.rangeMeter = rangeMeter;
            this.saveTexture = saveTexture;
            this.map_type = map_type;
            this.useChunkOnly = useChunkOnly;
            this.maxMemoryUsage = maxMemoryUsage;
            this.reverseVertexOrder = reverseVertexOrder;
            this.stabilityCounter = stabilityCounter;
        }
        /// <summary>
        /// Returns the resolution corresponding to the given MAPPING_RESOLUTION preset.
        /// </summary>
        /// <param name="mappingResolution">The desired MAPPING_RESOLUTION.</param>
        /// <returns>The resolution in meters.</returns>
        public static float get(MAPPING_RESOLUTION mappingResolution = MAPPING_RESOLUTION.MEDIUM)
        {
            if (mappingResolution == MAPPING_RESOLUTION.HIGH)
            {
                return 0.05f;
            }
            else if (mappingResolution == MAPPING_RESOLUTION.MEDIUM)
            {
                return 0.10f;
            }
            if (mappingResolution == MAPPING_RESOLUTION.LOW)
            {
                return 0.15f;
            }
            return 0.10f;
        }
        /// <summary>
        /// Returns the resolution corresponding to the given MAPPING_RANGE preset.
        /// </summary>
        /// <param name="mappingRange">The desired MAPPING_RANGE. Default: MAPPING_RANGE::MEDIUM.</param>
        /// <returns>The range in meters.</returns>
        public static float get(MAPPING_RANGE mappingRange = MAPPING_RANGE.MEDIUM)
        {
            if (mappingRange == MAPPING_RANGE.NEAR)
            {
                return 3.5f;
            }
            else if (mappingRange == MAPPING_RANGE.MEDIUM)
            {
                return 5.0f;
            }
            if (mappingRange == MAPPING_RANGE.FAR)
            {
                return 10.0f;
            }
            return 5.0f;
        }
        /// <summary>
        /// Sets the resolution corresponding to the given MAPPING_RESOLUTION preset.
        /// </summary>
        /// <param name="mappingResolution">The desired MAPPING_RESOLUTION.</param>
        public void set(MAPPING_RESOLUTION mappingResolution = MAPPING_RESOLUTION.MEDIUM)
        {
            if (mappingResolution == MAPPING_RESOLUTION.HIGH)
            {
                resolutionMeter = 0.05f;
            }
            else if (mappingResolution == MAPPING_RESOLUTION.MEDIUM)
            {
                resolutionMeter = 0.10f;
            }
            if (mappingResolution == MAPPING_RESOLUTION.LOW)
            {
                resolutionMeter = 0.15f;
            }
        }
        /// <summary>
        /// Sets the maximum value of the depth corresponding to the given MAPPING_RANGE preset.
        /// </summary>
        /// <param name="mappingRange">The desired MAPPING_RANGE. Default: MAPPING_RANGE::MEDIUM.</param>
        public void set(MAPPING_RANGE mappingRange = MAPPING_RANGE.MEDIUM)
        {
            if (mappingRange == MAPPING_RANGE.NEAR)
            {
                rangeMeter = 3.5f;
            }
            else if (mappingRange == MAPPING_RANGE.MEDIUM)
            {
                rangeMeter = 5.0f;
            }
            if (mappingRange == MAPPING_RANGE.FAR)
            {
                rangeMeter = 10.0f;
            }
        }
    }

    ///\ingroup SpatialMapping_group
    /// <summary>
    /// Spatial mapping depth resolution presets.
    /// </summary>
    public enum MAPPING_RESOLUTION
    {
        /// <summary>
        /// Create detailed geometry. Requires lots of memory.
        /// </summary>
        HIGH,
        /// <summary>
        /// Small variations in the geometry will disappear. Useful for large objects.
        /// </summary>
        ///
        MEDIUM,
        /// <summary>
        /// Keeps only large variations of the geometry. Useful for outdoors.
        /// </summary>
        LOW
    }

    ///\ingroup SpatialMapping_group
    /// <summary>
    ///  Spatial mapping depth range presets.
    /// </summary>
    public enum MAPPING_RANGE
    {
        /// <summary>
        /// Geometry within 3.5 meters of the camera will be mapped.
        /// </summary>
        NEAR,
        /// <summary>
        /// Geometry within 5 meters of the camera will be mapped.
        /// </summary>
        MEDIUM,
        /// <summary>
        /// Objects as far as 10 meters away are mapped. Useful for outdoors.
        /// </summary>
        FAR
    }

    ///\ingroup SpatialMapping_group
    /// <summary>
    /// Spatial Mapping type (default is mesh)
    /// </summary>m
    public enum SPATIAL_MAP_TYPE
    {
        /// <summary>
        /// Represent a surface with faces, 3D points are linked by edges, no color information
        /// </summary>
        MESH,
        /// <summary>
        ///  Geometry is represented by a set of 3D colored points.
        /// </summary>
        FUSED_POINT_CLOUD
    };

    ///\ingroup SpatialMapping_group
    /// <summary>
    /// Mesh formats that can be saved/loaded with spatial mapping.
    /// </summary>
    public enum MESH_FILE_FORMAT
    {
        /// <summary>
        /// Contains only vertices and faces.
        /// </summary>
        PLY,
        /// <summary>
        /// Contains only vertices and faces, encoded in binary.
        /// </summary>
        BIN,
        /// <summary>
        /// Contains vertices, normals, faces, and texture information (if possible).
        /// </summary>
        OBJ
    }

    ///\ingroup SpatialMapping_group
    /// <summary>
    /// Presets for filtering meshes scannedw ith spatial mapping. Higher values reduce total face count by more.
    /// </summary>
    public enum MESH_FILTER
    {
        /// <summary>
        /// Soft decimation and smoothing.
        /// </summary>
        LOW,
        /// <summary>
        /// Decimate the number of faces and apply a soft smooth.
        /// </summary>
        MEDIUM,
        /// <summary>
        /// Drastically reduce the number of faces.
        /// </summary>
        HIGH,
    }

    ///\ingroup SpatialMapping_group
    /// <summary>
    /// Possible states of the ZED's Spatial Mapping system.
    /// </summary>
    public enum SPATIAL_MAPPING_STATE
    {
        /// <summary>
        /// Spatial mapping is initializing.
        /// </summary>
        INITIALIZING,
        /// <summary>
        /// Depth and tracking data were correctly integrated into the fusion algorithm.
        /// </summary>
        OK,
        /// <summary>
        /// Maximum memory dedicated to scanning has been reached; the mesh will no longer be updated.
        /// </summary>
        NOT_ENOUGH_MEMORY,
        /// <summary>
        /// EnableSpatialMapping() wasn't called (or the scanning was stopped and not relaunched).
        /// </summary>
        NOT_ENABLED,
        /// <summary>
        /// Effective FPS is too low to give proper results for spatial mapping.
        /// Consider using performance-friendly parameters (DEPTH_MODE_PERFORMANCE, VGA or HD720 camera resolution,
        /// and LOW spatial mapping resolution).
        /// </summary>
        FPS_TOO_LOW
    }

    ///\ingroup Core_group
    /// <summary>
    /// Units used by the SDK for measurements and tracking.
    /// </summary>
    public enum UNIT
    {
        /// <summary>
        /// International System, 1/1000 meters.
        /// </summary>
        MILLIMETER,
        /// <summary>
        /// International System, 1/100 meters.
        /// </summary>
        CENTIMETER,
        /// <summary>
        /// International System, 1/1 meters.
        /// </summary>
        METER,
        /// <summary>
        ///  Imperial Unit, 1/12 feet.
        /// </summary>
        INCH,
        /// <summary>
        ///  Imperial Unit, 1/1 feet.
        /// </summary>
        FOOT
    }

    ///\ingroup SpatialMapping_group
    /// <summary>
    /// Type of the plane, determined by its orientation and whether detected by ZEDPlaneDetectionManager's
    /// DetectFloorPlane() or DetectPlaneAtHit().
    /// </summary>
    public enum PLANE_TYPE
    {
        /// <summary>
        /// Floor plane of a scene. Retrieved by ZEDPlaneDetectionManager.DetectFloorPlane().
        /// </summary>
        FLOOR,
        /// <summary>
        /// Horizontal plane, such as a tabletop, floor, etc. Detected with DetectPlaneAtHit() using screen-space coordinates.
        /// </summary>
        HIT_HORIZONTAL,
        /// <summary>
        /// Vertical plane, such as a wall. Detected with DetectPlaneAtHit() using screen-space coordinates.
        /// </summary>
        HIT_VERTICAL,
        /// <summary>
        /// Plane at an angle neither parallel nor perpendicular to the floor. Detected with DetectPlaneAtHit() using screen-space coordinates.
        /// </summary>
        HIT_UNKNOWN
    };

    ///\ingroup SpatialMapping_group
    /// <summary>
    ///  Possible states of the ZED's spatial memory area export, for saving 3D features used
    ///  by the tracking system to relocalize the camera. This is used when saving a mesh generated
    ///  by spatial mapping when Save Mesh is enabled - a .area file is saved as well.
    /// </summary>
    public enum AREA_EXPORT_STATE
    {
        /// <summary>
        /// Spatial memory file has been successfully created.
        /// </summary>
        AREA_EXPORT_STATE_SUCCESS,
        /// <summary>
        /// Spatial memory file is currently being written to.
        /// </summary>
        AREA_EXPORT_STATE_RUNNING,
        /// <summary>
        /// Spatial memory file export has not been called.
        /// </summary>
        AREA_EXPORT_STATE_NOT_STARTED,
        /// <summary>
        /// Spatial memory contains no data; the file is empty.
        /// </summary>
        AREA_EXPORT_STATE_FILE_EMPTY,
        /// <summary>
        /// Spatial memory file has not been written to because of a bad file name.
        /// </summary>
        AREA_EXPORT_STATE_FILE_ERROR,
        /// <summary>
        /// Spatial memory has been disabled, so no file can be created.
        /// </summary>
        AREA_EXPORT_STATE_SPATIAL_MEMORY_DISABLED
    };

    ///\ingroup SpatialMapping_group
    /// <summary>
    /// A mesh contains the geometric (and optionally texture) data of the scene captured by spatial mapping.
    /// </summary>
    public class Mesh
    {
        /// <summary>
        /// Total vertices in each chunk/submesh.
        /// </summary>
        public int[] nbVerticesInSubmesh = new int[(int)Constant.MAX_SUBMESH];
        /// <summary>
        /// Total triangles in each chunk/submesh.
        /// </summary>
        public int[] nbTrianglesInSubmesh = new int[(int)Constant.MAX_SUBMESH];
        /// <summary>
        /// Total indices per chunk/submesh.
        /// </summary>
        public int[] updatedIndices = new int[(int)Constant.MAX_SUBMESH];
        /// <summary>
        /// Vertex count in current submesh.
        /// </summary>
        public int nbVertices = 0;
        /// <summary>
        /// Triangle point counds in current submesh. (Every three values are the indexes of the three vertexes that make up one triangle)
        /// </summary>
        public int nbTriangles = 0;
        /// <summary>
        /// How many submeshes were updated.
        /// </summary>
        public int nbUpdatedSubmesh = 0;
        /// <summary>
        /// Vertices of the mesh.
        /// </summary>
        public Vector3[] vertices = new Vector3[(int)Constant.MAX_SUBMESH];
        /// <summary>
        /// Contains the index of the vertices.
        /// </summary>
        public int[] triangles = new int[(int)Constant.MAX_SUBMESH];
        /// <summary>
        /// Contains the colors of the vertices.
        /// </summary>
        public byte[] colors = new byte[(int)Constant.MAX_SUBMESH];
        /// <summary>
        /// UVs defines the 2D projection of each vertices onto the Texture.
        /// Values are normalized [0;1], starting from the bottom left corner of the texture (as requested by opengl).
        /// In order to display a textured mesh you need to bind the Texture and then draw each triangles by picking its uv values.
        /// </summary>
        public Vector2[] uvs = null;
        /// <summary>
        /// Texture of the mesh
        /// </summary>
        public IntPtr textures = IntPtr.Zero;
        /// <summary>
        /// Width and height of the mesh texture, if any.
        /// </summary>
        public int[] texturesSize = new int[2];
        /// <summary>
        /// Dictionary of all existing chunks.
        /// </summary>
        public Dictionary<int, Chunk> chunks = new Dictionary<int, Chunk>((int)Constant.MAX_SUBMESH);
    }

    /// <summary>
    /// A fused point cloud contains both geometric and color data of the scene captured by spatial mapping.
    /// </summary>
    public class FusedPointCloud
    {
        /// <summary>
        /// Vertices are defined by colored 3D points {x, y, z, rgba}.
        /// </summary>
        public Vector4[] vertices;
    }

    ///\ingroup SpatialMapping_group
    /// <summary>
    /// Represents a sub-mesh, it contains local vertices and triangles.
    /// </summary>
    public struct Chunk
    {
        /// <summary>
        /// Vertices are defined by a 3D point {x,y,z}.
        /// </summary>
        public Vector3[] vertices;
        /// <summary>
        /// Triangles (or faces) contains the index of its three vertices. It corresponds to the 3 vertices of the triangle {v1, v2, v3}.
        /// </summary>
        public int[] triangles;
        /// <summary>
        /// Colors of the vertices.
        /// </summary>
        public byte[] colors;
    }

    /// <summary>
    /// Structure that defines a new plane, holding information directly from the ZED SDK.
    /// Data within is relative to the camera;
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct PlaneData
    {
        /// <summary>
        /// Error code returned by the ZED SDK when the plane detection was attempted.
        /// </summary>
        public sl.ERROR_CODE ErrorCode;
        /// <summary>
        /// Type of the plane (floor, hit_vertical, etc.)
        /// </summary>
        public PLANE_TYPE Type;
        /// <summary>
        /// Normalized vector of the direction the plane is facing.
        /// </summary>
        public Vector3 PlaneNormal;
        /// <summary>
        /// Camera-space position of the center of the plane.
        /// </summary>
        public Vector3 PlaneCenter;
        /// <summary>
        /// Camera-space position of the center of the plane.
        /// </summary>
        public Vector3 PlaneTransformPosition;
        /// <summary>
        /// Camera-space rotation/orientation of the plane.
        /// </summary>
        public Quaternion PlaneTransformOrientation;
        /// <summary>
        /// The mathematical Vector4 equation of the plane.
        /// </summary>
        public Vector4 PlaneEquation;
        /// <summary>
        /// How wide and long/tall the plane is in meters.
        /// </summary>
        public Vector2 Extents;
        /// <summary>
        /// How many points make up the plane's bounds, eg. the array length of Bounds.
        /// </summary>
        public int BoundsSize;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        ///Positions of the points that make up the edges of the plane's mesh.
        public Vector3[] Bounds; //max 256 points
    }

    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////  Object Detection /////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region Object Detection Module

    /// \ingroup Object_group
    /// <summary>
    /// sets batch trajectory parameters
    /// The default constructor sets all parameters to their default settings.
    /// Parameters can be user adjusted.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct BatchParameters
    {
        /// <summary>
        /// Defines if the Batch option in the object detection module is enabled. Batch queueing system provides:
        ///  - Deep-Learning based re-identification
        /// - Trajectory smoothing and filtering
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool enable;
        /// <summary>
        /// Max retention time in seconds of a detected object. After this time, the same object will mostly have a different ID.
        /// </summary>
        public float idRetentionTime;
        /// <summary>
        /// Trajectories will be output in batch with the desired latency in seconds.
        /// During this waiting time, re-identification of objects is done in the background.
        /// Specifying a short latency will limit the search (falling in timeout) for previously seen object IDs but will be closer to real time output.
        /// Specifying a long latency will reduce the change of timeout in Re-ID but increase difference with live output.
        /// </summary>
        public float latency;
    }


    /// <summary>
    /// Contains AI model status.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct AI_Model_status
    {
        /// <summary>
        /// The model file is currently present on the host.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool downloaded;
        /// <summary>
        /// An engine file with the expected architecture is found.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool optimized;
    };

    ///\ingroup Object_group
    /// <summary>
    /// Sets the object detection parameters.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ObjectDetectionParameters
    {
        /// <summary>
        /// Defines a module instance id. This is used to identify which object detection model instance is used.
        /// </summary>
        uint instanceModuleId;
        /// <summary>
        /// Defines if the object detection is synchronized to the image or runs in a separate thread.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool imageSync;
        /// <summary>
        /// Defines if the object detection will track objects across multiple images, instead of an image-by-image basis.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool enableObjectTracking;
        /// <summary>
        /// Defines if the SDK will calculate 2D masks for each object. Requires more performance, so don't enable if you don't need these masks.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool enableSegmentation;
        /// <summary>
        /// Defines the AI model used for detection
        /// </summary>
        public sl.OBJECT_DETECTION_MODEL detectionModel;

        /// <summary>
        /// Defines a upper depth range for detections.
        /// Defined in  UNIT set at  sl.Camera.Open.
        /// Default value is set to sl.Initparameters.depthMaximumDistance (can not be higher).
        /// </summary>
        public float maxRange;
        /// <summary>
        /// Batching system parameters.
        /// Batching system(introduced in 3.5) performs short-term re-identification with deep learning and trajectories filtering.
        /// BatchParameters.enable need to be true to use this feature (by default disabled)
        /// </summary>
        public BatchParameters batchParameters;

        /// <summary>
        /// Defines the filtering mode that should be applied to raw detections.
        /// </summary>
        public OBJECT_FILTERING_MODE filteringMode;
        /// <summary>
        /// When an object is not detected anymore, the SDK will predict its positions during a short period of time before switching its state to SEARCHING.
	    /// It prevents the jittering of the object state when there is a short misdetection.The user can define its own prediction time duration.
	    /// During this time, the object will have OK state even if it is not detected.
	    /// The duration is expressed in seconds.
        /// The prediction_timeout_s will be clamped to 1 second as the prediction is getting worst with time.
	    /// Set this parameter to 0 to disable SDK predictions.
        /// </summary>
        public float predictionTimeout_s;

        /// <summary>
        /// Allow inference to run at a lower precision to improve runtime and memory usage,
        /// it might increase the initial optimization time and could include downloading calibration data or calibration cache and slightly reduce the accuracy.
	    /// Note: The fp16 is automatically enabled if the GPU is compatible and provides a speed up of almost x2 and reduce memory usage by /a    lmost //half, no precision loss.
	    /// Note: This setting allow int8 precision which can speed up by another x2 factor (compared to fp16, or x4 compared to fp32) and half the fp16 memory usage, however some accuracy can be lost.
        /// The accuracy loss should not exceed 1-2% on the compatible models.
	    /// The current compatible models are all HUMAN_BODY_XXXX
        /// </summary>
        public bool allowReducedPrecisionInference;
    };

    ///\ingroup Object_group
    /// <summary>
    /// Sets the object detection runtime parameters.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ObjectDetectionRuntimeParameters
    {
        /// <summary>
        /// The detection confidence threshold between 1 and 99.
        /// A confidence of 1 means a low threshold, more uncertain objects and 99 very few but very precise objects.
        /// Ex: If set to 80, then the SDK must be at least 80% sure that a given object exists before reporting it in the list of detected objects.
        /// If the scene contains a lot of objects, increasing the confidence can slightly speed up the process, since every object instance is tracked.
        /// Default confidence threshold value, used as a fallback when ObjectDetectionRuntimeParameters.object_confidence_threshold is partially set
        /// </summary>
        public float detectionConfidenceThreshold;
        /// <summary>
        /// Select which object type to detect and track. Fewer objects type can slightly speed up the process, since every objects are tracked. Only the selected classes in the vector will be outputted.
        /// In order to get all the available classes, the filter vector must be empty :
        /// <c> object_class_filter = new int[(int)sl.OBJECT_CLASS.LAST)]; </c>
        /// To select a set of specific object classes, like person and vehicle for instance:
        /// objectClassFilter[(int)sl.OBJECT_CLASS.PERSON] = Convert.ToInt32(true);
        /// objectClassFilter[(int)sl.OBJECT_CLASS.VEHICLE] = Convert.ToInt32(true);
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)sl.OBJECT_CLASS.LAST)]
        public int[] objectClassFilter;

        /// <summary>
        /// Defines a detection threshold for each classes, can be empty for some classes, ObjectDetectionRuntimeParameters.detectionConfidenceThreshold will be taken as fallback/default value.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)sl.OBJECT_CLASS.LAST)]
        public int[] objectConfidenceThreshold;
        /// <summary>
        /// Defines the minimum keypoints threshold.
	    /// the SDK will outputs skeletons with more keypoints than this threshold
        /// it is useful for example to remove unstable fitting results when a skeleton is partially occluded
        /// </summary>
        public int minimumKeypointsThreshold;
    };

    ///\ingroup Object_group
    /// <summary>
    /// Sets the body tracking parameters.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct BodyTrackingParameters
    {
        /// <summary>
        /// Defines a module instance id. This is used to identify which object detection model instance is used.
        /// </summary>
        uint instanceModuleId;
        /// <summary>
        /// Defines if the object detection is synchronized to the image or runs in a separate thread.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool imageSync;
        /// <summary>
        /// Defines if the object detection will track objects across multiple images, instead of an image-by-image basis.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool enableObjectTracking;
        /// <summary>
        /// Defines if the SDK will calculate 2D masks for each object. Requires more performance, so don't enable if you don't need these masks.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool enableSegmentation;
        /// <summary>
        /// Defines the AI model used for detection
        /// </summary>
        public sl.BODY_TRACKING_MODEL detectionModel;
        /// <summary>
        /// Defines if the body fitting will be applied.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool enableBodyFitting;
        /// <summary>
        /// Body Format. BODY_FORMAT.POSE_34 automatically enables body fitting.
        /// </summary>
        public sl.BODY_FORMAT bodyFormat;
        /// <summary>
        /// Defines a upper depth range for detections.
        /// Defined in  UNIT set at  sl.Camera.Open.
        /// Default value is set to sl.Initparameters.depthMaximumDistance (can not be higher).
        /// </summary>
        public float maxRange;

#if false
        /// <summary>
        /// Batching system parameters.
        /// Batching system(introduced in 3.5) performs short-term re-identification with deep learning and trajectories filtering.
        /// BatchParameters.enable need to be true to use this feature (by default disabled)
        /// </summary>
        public BatchParameters batchParameters;
#endif
        /// <summary>
        /// When an object is not detected anymore, the SDK will predict its positions during a short period of time before switching its state to SEARCHING.
	    /// It prevents the jittering of the object state when there is a short misdetection.The user can define its own prediction time duration.
	    /// During this time, the object will have OK state even if it is not detected.
	    /// The duration is expressed in seconds.
        /// The prediction_timeout_s will be clamped to 1 second as the prediction is getting worst with time.
	    /// Set this parameter to 0 to disable SDK predictions.
        /// </summary>
        public float predictionTimeout_s;

        /// <summary>
        /// Allow inference to run at a lower precision to improve runtime and memory usage,
        /// it might increase the initial optimization time and could include downloading calibration data or calibration cache and slightly reduce the accuracy.
	    /// Note: The fp16 is automatically enabled if the GPU is compatible and provides a speed up of almost x2 and reduce memory usage by /a    lmost //half, no precision loss.
	    /// Note: This setting allow int8 precision which can speed up by another x2 factor (compared to fp16, or x4 compared to fp32) and half the fp16 memory usage, however some accuracy can be lost.
        /// The accuracy loss should not exceed 1-2% on the compatible models.
	    /// The current compatible models are all HUMAN_BODY_XXXX
        /// </summary>
        public bool allowReducedPrecisionInference;
    };

    ///\ingroup Object_group
    /// <summary>
    /// Sets the object detection runtime parameters.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct BodyTrackingRuntimeParameters
    {
        /// <summary>
        /// The detection confidence threshold between 1 and 99.
        /// A confidence of 1 means a low threshold, more uncertain objects and 99 very few but very precise objects.
        /// Ex: If set to 80, then the SDK must be at least 80% sure that a given object exists before reporting it in the list of detected objects.
        /// If the scene contains a lot of objects, increasing the confidence can slightly speed up the process, since every object instance is tracked.
        /// </summary>
        public float detectionConfidenceThreshold;

        /// <summary>
        /// Defines the minimum keypoints threshold.
	    /// the SDK will outputs skeletons with more keypoints than this threshold
        /// it is useful for example to remove unstable fitting results when a skeleton is partially occluded
        /// </summary>
        public int minimumKeypointsThreshold;

        /// <summary>
        ///  This value controls the smoothing of the fitted fused skeleton.
        /// it is ranged from 0 (low smoothing) and 1 (high smoothing)
        /// Default is 0
        /// </summary>
        public float skeletonSmoothing;
    };

    ///\ingroup Object_group
	/// <summary>
	/// Object data structure directly from the SDK. Represents a single object detection.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
    public struct ObjectData
    {
        /// <summary>
        /// Object identification number, used as a reference when tracking the object through the frames.
        /// </summary>
        public int id; 
        /// <summary>
        ///Unique ID to help identify and track AI detections. Can be either generated externally, or using \ref ZEDCamera.generateUniqueId() or left empty
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 37)]
        public string uniqueObjectId;
        /// <summary>
        ///  Object label, forwarded from \ref CustomBoxObjects when using DETECTION_MODEL.CUSTOM_BOX_OBJECTS
        /// </summary>
        public int rawLabel;
        /// <summary>
        /// Object category. Identify the object type.
        /// </summary>
		public sl.OBJECT_CLASS label;
        /// <summary>
        /// Object subclass.
        /// </summary>
        public sl.OBJECT_SUBCLASS sublabel;
        /// <summary>
        /// Defines the object tracking state.
        /// </summary>
		public sl.OBJECT_TRACKING_STATE objectTrackingState;
        /// <summary>
        /// Defines the object action state.
        /// </summary>
		public sl.OBJECT_ACTION_STATE actionState;
        /// <summary>
        /// Defines the object 3D centroid.
        /// </summary>
        public Vector3 position;
        /// <summary>
        /// Defines the detection confidence value of the object. A lower confidence value means the object might not be localized perfectly or the label (OBJECT_CLASS) is uncertain.
        /// </summary>
		public float confidence;
        /// <summary>
        /// Defines for the bounding_box_2d the pixels which really belong to the object (set to 255) and those of the background (set to 0).
        /// </summary>
		public System.IntPtr mask;
        /// <summary>
        /// Image data.
        /// Note that Y in these values is relative from the top of the image.
        /// If using this raw value, subtract Y from the
        /// image height to get the height relative to the bottom.
        /// </summary>
        ///  0 ------- 1
        ///  |   obj   |
        ///  3-------- 2
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Vector2[] boundingBox2D;
        /// <summary>
        /// 3D head centroid.
	    /// Note: Not available with DETECTION_MODEL::MULTI_CLASS_BOX.
        /// </summary>
		public Vector3 headPosition; //object head position (only for HUMAN detectionModel)
        /// <summary>
        /// Defines the object 3D velocity.
        /// </summary>
		public Vector3 velocity; //object root velocity
        /// <summary>
        /// 3D object dimensions: width, height, length. Defined in InitParameters.UNIT, expressed in RuntimeParameters.measure3DReferenceFrame.
        /// </summary>
        public Vector3 dimensions;
        /// <summary>
        /// The 3D space bounding box. given as array of vertices
        /// </summary>
        ///   1 ---------2
        ///  /|         /|
        /// 0 |--------3 |
        /// | |        | |
        /// | 5--------|-6
        /// |/         |/
        /// 4 ---------7
        ///
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public Vector3[] boundingBox; // 3D Bounding Box of object
        /// <summary>
        /// bounds the head with eight 3D points.
        /// </summary>
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public Vector3[] headBoundingBox;// 3D Bounding Box of head (only for HUMAN detectionModel)
        /// <summary>
        /// bounds the head with four 3D points.
        /// </summary>
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public Vector2[] headBoundingBox2D;// 2D Bounding Box of head
        /// <summary>
        /// Full covariance matrix for position (3x3). Only 6 values are necessary
        /// [p0, p1, p2]
        /// [p1, p3, p4]
        /// [p2, p4, p5]
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public float[] positionCovariance;// covariance matrix of the 3d position, represented by its upper triangular matrix value
    };

    ///\ingroup Object_group
    /// <summary>
    /// Container to store the externally detected objects. The objects can be ingested using IngestCustomBoxObjects() function to extract 3D information and tracking over time.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CustomBoxObjectData
    {
        /// <summary>
        ///Unique ID to help identify and track AI detections. Can be either generated externally, or using \ref ZEDCamera.generateUniqueId() or left empty
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 37)]
        public string uniqueObjectID;
        /// <summary>
        /// 2D bounding box represented as four 2D points starting at the top left corner and rotation clockwise.
        /// </summary>
        ///  0 ------- 1
        ///  |   obj   |
        ///  3-------- 2
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Vector2[] boundingBox2D;
        /// <summary>
        /// Object label, this information is passed-through and can be used to improve object tracking
        /// </summary>
        public int label;
        /// <summary>
        /// Detection confidence. Should be [0-1]. It can be used to improve the object tracking
        /// </summary>
        public float probability;
        /// <summary>
        /// Provide hypothesis about the object movements(degrees of freedom) to improve the object tracking
        /// true: means 2 DoF projected alongside the floor plane, the default for object standing on the ground such as person, vehicle, etc
        /// false : 6 DoF full 3D movements are allowed
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool isGrounded;
    }

    ///\ingroup Object_group
    /// <summary>
    /// Object Scene data directly from the ZED SDK. Represents all detections given during a single image frame.
    /// Contains the number of object in the scene and the objectData structure for each object.
    /// Since the data is transmitted from C++ to C#, the size of the structure must be constant. Therefore, there is a limitation of 200 (MAX_OBJECT constant) objects in the image.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Objects
    {
        /// <summary>
        /// How many objects were detected this frame. Use this to iterate through the top of objectData; objects with indexes greater than numObject are empty.
        /// </summary>
        public int numObject;
        /// <summary>
        /// Timestamp of the image where these objects were detected.
        /// </summary>
        public ulong timestamp;
        /// <summary>
        /// Defines if the object frame is new (new timestamp)
        /// </summary>
        public int isNew;
        /// <summary>
        /// Defines if the object is tracked
        /// </summary>
        public int isTracked;
        /// <summary>
        /// Current detection model used.
        /// </summary>
        public sl.OBJECT_DETECTION_MODEL detectionModel;
        /// <summary>
        /// Array of objects
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)(Constant.MAX_OBJECTS))]
        public ObjectData[] objectData;

        /// <summary>
        /// Function that look for a given object ID in the current object list and return the object associated if found and a status.
        /// </summary>
        /// <param name="objectData">[out] : The object corresponding to the given ID if found</param>
        /// <param name="objectDataId">  The input object ID</param>
        /// <returns> True if found False otherwise</returns>
        public bool GetObjectDataFromId(ref sl.ObjectData objectData, int objectDataId)
        {
            bool output = false;
            objectData = new sl.ObjectData();
            for (int idx = 0; idx < this.numObject; idx++)
                if (this.objectData[idx].id == 0)
                {
                    objectData = this.objectData[idx];
                    output = true;
                }
            return output;
        }
    };

    /// <summary>
    /// Full covariance matrix for position (3x3). Only 6 values are necessary
    /// [p0, p1, p2]
    /// [p1, p3, p4]
    /// [p2, p4, p5]
    /// </summary>
    public struct CovarMatrix
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public float[] values;// covariance matrix of the 3d position, represented by its upper triangular matrix value
    };

    ///\ingroup Object_group
	/// <summary>
	/// Object data structure directly from the SDK. Represents a single object detection.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
    public struct BodyData
    {
        /// <summary>
        /// Object identification number, used as a reference when tracking the object through the frames.
        /// </summary>
        public int id;
        /// <summary>
        ///Unique ID to help identify and track AI detections. Can be either generated externally, or using \ref ZEDCamera.generateUniqueId() or left empty
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 37)]
        public string uniqueObjectId;
        /// <summary>
        /// Defines the object tracking state.
        /// </summary>
		public sl.OBJECT_TRACKING_STATE trackingState;
        /// <summary>
        /// Defines the object action state.
        /// </summary>
		public sl.OBJECT_ACTION_STATE actionState;
        /// <summary>
        /// Defines the object 3D centroid.
        /// </summary>
        public Vector3 position;
        /// <summary>
        /// Defines the object 3D velocity.
        /// </summary>
		public Vector3 velocity; //object root velocity
        /// <summary>
        /// Full covariance matrix for position (3x3)
        /// </summary>
        public CovarMatrix positionCovariance;// covariance matrix of the 3d position, represented by its upper triangular matrix value
        /// <summary>
        /// Defines the detection confidence value of the object. A lower confidence value means the object might not be localized perfectly or the label (OBJECT_CLASS) is uncertain.
        /// </summary>
		public float confidence;
        /// <summary>
        /// Defines for the bounding_box_2d the pixels which really belong to the object (set to 255) and those of the background (set to 0).
        /// </summary>
		public System.IntPtr mask;
        /// <summary>
        /// Image data.
        /// Note that Y in these values is relative from the top of the image.
        /// If using this raw value, subtract Y from the
        /// image height to get the height relative to the bottom.
        /// </summary>
        ///  0 ------- 1
        ///  |   obj   |
        ///  3-------- 2
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Vector2[] boundingBox2D;
        /// <summary>
        /// 3D head centroid.
	    /// Note: Not available with DETECTION_MODEL::MULTI_CLASS_BOX.
        /// </summary>
		public Vector3 headPosition; //object head position (only for HUMAN detectionModel)
        /// <summary>
        /// 3D object dimensions: width, height, length. Defined in InitParameters.UNIT, expressed in RuntimeParameters.measure3DReferenceFrame.
        /// </summary>
        public Vector3 dimensions;
        /// <summary>
        /// The 3D space bounding box. given as array of vertices
        /// </summary>
        ///   1 ---------2
        ///  /|         /|
        /// 0 |--------3 |
        /// | |        | |
        /// | 5--------|-6
        /// |/         |/
        /// 4 ---------7
        ///
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public Vector3[] boundingBox; // 3D Bounding Box of object
        /// <summary>
        /// bounds the head with eight 3D points.
        /// </summary>
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public Vector3[] headBoundingBox;// 3D Bounding Box of head (only for HUMAN detectionModel)
        /// <summary>
        /// bounds the head with four 3D points.
        /// </summary>
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public Vector2[] headBoundingBox2D;// 2D Bounding Box of head
        /// <summary>
        /// A set of useful points representing the human body, expressed in 2D. We use a classic 18 points representation, the points semantic and order is given by BODY_PARTS.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 38)]
        public Vector2[] keypoints2D;
        /// <summary>
        /// A set of useful points representing the human body, expressed in 3D. We use a classic 18 points representation, the points semantic and order is given by BODY_PARTS.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 38)]
        public Vector3[] keypoints;// 3D position of the joints of the skeleton

        /// <summary>
        ///  Per keypoint detection confidence, can not be lower than the \ref ObjectDetectionRuntimeParameters.detectionConfidenceThreshold.
        ///  Not available with DETECTION_MODEL.MULTI_CLASS_BOX.
        ///  in some cases, eg. body partially out of the image or missing depth data, some keypoint can not be detected, they will have non finite values.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 38)]
        public float[] keypointConfidence;

        /// <summary>
        ///  Per keypoint detection confidence, can not be lower than the \ref ObjectDetectionRuntimeParameters.detectionConfidenceThreshold.
        ///  Not available with DETECTION_MODEL.MULTI_CLASS_BOX.
        ///  in some cases, eg. body partially out of the image or missing depth data, some keypoint can not be detected, they will have non finite values.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 38)]
        public CovarMatrix[] keypointCovariances;

        /// <summary>
        /// Global position per joint in the coordinate frame of the requested skeleton format.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 38)]
        public Vector3[] localPositionPerJoint;
        /// <summary>
        /// Local orientation per joint in the coordinate frame of the requested skeleton format.
        /// The orientation is represented by a quaternion.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 38)]
        public Quaternion[] localOrientationPerJoint;
        /// <summary>
        /// Global root position.
        /// </summary>
        public Quaternion globalRootOrientation;
    };

    /// <summary>
    /// Contains the result of the body detection module.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct Bodies
    {
        /// <summary>
        /// Number of detected bodies. Used to iterate through the object_list array.
        /// </summary>
        public int nbBodies;
        /// <summary>
        /// Defines the timestamp corresponding to the frame acquisition.
        /// This value is especially useful for the async mode to synchronize the data.
        /// </summary>
        public ulong timestamp;

        /// <summary>
        /// Defined if the object list has already been retrieved or not.
        /// </summary>
        public int isNew;

        /// <summary>
        /// True if both the object tracking and the world orientation has been setup.
        /// </summary>
        public int isTracked;

        /// <summary>
        /// The list of detected bodies.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)(Constant.MAX_OBJECTS))]
        public BodyData[] bodiesList;

    }

    ///\ingroup Object_group
    /// <summary>
    /// Lists of supported skeleton body model
    /// </summary>
    public enum BODY_FORMAT
    {
        /// <summary>
        /// Simple body model, including simplified face and tracking wrists and ankle positions.
        /// No hands nor feet tracking available with this model.
        /// </summary>
        BODY_18,
        /// <summary>
        /// Body model, including feet, simplified face and simplified hands.
        /// This body model does not provide wrist/ankle rotations.
        /// </summary>
        BODY_34,
        /// <summary>
        /// Body model, including feet, simplified face and simplified hands
        /// </summary>
        BODY_38,
#if false
        /// <summary>
        /// Body model, including feet, simplified face and detailed hands
        /// </summary>
        BODY_70
#endif     
        };

    public enum BODY_KEYPOINTS_SELECTION
    {
        /// <summary>
        /// Full keypoints model.
        /// </summary>
        FULL,
        /// <summary>
        /// Upper body keypoint model. Only the upper body will be outputted (from hips up).
        /// </summary>
        UPPER_BODY
    };

    ///\ingroup Object_group
    /// <summary>
    /// Lists available object class
    /// </summary>
    public enum OBJECT_CLASS
    {
        PERSON = 0,
        VEHICLE = 1,
        BAG = 2,
        ANIMAL = 3,
        ELECTRONICS = 4,
        FRUIT_VEGETABLE = 5,
        SPORT = 6,
        LAST = 7
    };

    ///\ingroup Object_group
    /// <summary>
    /// Lists available object subclass.
    /// </summary>
    public enum OBJECT_SUBCLASS
    {
        PERSON = 0,
        // VEHICLES
        BICYCLE = 1,
        CAR = 2,
        MOTORBIKE = 3,
        BUS = 4,
        TRUCK = 5,
        BOAT = 6,
        // BAGS
        BACKPACK = 7,
        HANDBAG = 8,
        SUITCASE = 9,
        // ANIMALS
        BIRD = 10,
        CAT = 11,
        DOG = 12,
        HORSE = 13,
        SHEEP = 14,
        COW = 15,
        // ELECTRONICS
        CELLPHONE = 16,
        LAPTOP = 17,
        // FRUITS/VEGETABLES
        BANANA = 18,
        APPLE = 19,
        ORANGE = 20,
        CARROT = 21,
        PERSON_HEAD = 22,
        SPORTSBALL = 23,
        LAST = 24
    };

    ///\ingroup Object_group
    /// <summary>
    /// Tracking state of an individual object.
    /// </summary>
    public enum OBJECT_TRACKING_STATE
    {
        /// <summary>
        /// The tracking is not yet initialized, the object ID is not usable.
        /// </summary>
		OFF,
        /// <summary>
        /// The object is tracked.
        /// </summary>
		OK,
        /// <summary>
        /// The object couldn't be detected in the image and is potentially occluded, the trajectory is estimated.
        /// </summary>
		SEARCHING,
        /// <summary>
        /// This is the last searching state of the track, the track will be deleted in the next retreiveObject.
        /// </summary>
        TERMINATE
    };

    ///\ingroup Object_group
    /// <summary>
    /// Lists available object action state.
    /// </summary>
	public enum OBJECT_ACTION_STATE
    {
        /// <summary>
        /// The object is staying static.
        /// </summary>
		IDLE = 0,
        /// <summary>
        /// The object is moving.
        /// </summary>
		MOVING = 1
    };

    ///\ingroup Object_group
	/// <summary>
	/// List of available models for object detection
	/// </summary>
	public enum OBJECT_DETECTION_MODEL {
        /// <summary>
        /// Any objects, bounding box based.
        /// </summary>
		MULTI_CLASS_BOX_FAST,
        /// <summary>
        /// Any objects, bounding box based.
        /// </summary>
        MULTI_CLASS_BOX_MEDIUM,
        /// <summary>
        /// Any objects, bounding box based.
        /// </summary>
        MULTI_CLASS_BOX_ACCURATE,
        /// <summary>
        ///  Bounding Box detector specialized in person heads, particulary well suited for crowded environement, the person localization is also improved
        /// </summary>
        PERSON_HEAD_BOX_FAST,
        /// <summary>
        ///  Bounding Box detector specialized in person heads, particulary well suited for crowded environement, the person localization is also improved, state of the art accuracy
        /// </summary>
        PERSON_HEAD_BOX_ACCURATE,
        /// <summary>
        /// For external inference, using your own custom model and/or frameworks. This mode disable the internal inference engine, the 2D bounding box detection must be provided
        /// </summary>
        CUSTOM_BOX_OBJECTS
    };

    ///\ingroup Object_group
	/// <summary>
	/// List of available models for body tracking
	/// </summary>
	public enum BODY_TRACKING_MODEL
    {
        /// <summary>
        /// Keypoints based, specific to human skeleton, real time performance even on Jetson or low end GPU cards.
        /// </summary>
        HUMAN_BODY_FAST,
        /// <summary>
        ///  Keypoints based, specific to human skeleton, state of the art accuracy, requires powerful GPU.
        /// </summary>
		HUMAN_BODY_MEDIUM,
        /// <summary>
        /// Keypoints based, specific to human skeleton, real time performance even on Jetson or low end GPU cards.
        /// </summary>
        HUMAN_BODY_ACCURATE
    };

    public enum AI_MODELS
    {
        /// <summary>
        /// related to sl.DETECTION_MODEL.MULTI_CLASS_BOX
        /// </summary>
        MULTI_CLASS_DETECTION,
        /// <summary>
        /// related to sl.DETECTION_MODEL.MULTI_CLASS_BOX_MEDIUM
        /// </summary>
        MULTI_CLASS_MEDIUM_DETECTION,
        /// <summary>
        /// related to sl.DETECTION_MODEL.MULTI_CLASS_BOX_ACCURATE
        /// </summary>
        MULTI_CLASS_ACCURATE_DETECTION,
        /// <summary>
        /// related to sl.DETECTION_MODEL.HUMAN_BODY_FAST
        /// </summary>
        HUMAN_BODY_FAST_DETECTION,
        /// <summary>
        /// related to sl.DETECTION_MODEL.HUMAN_BODY_MEDIUM
        /// </summary>
        HUMAN_BODY_MEDIUM_DETECTION,
        /// <summary>
        /// related to sl.DETECTION_MODEL.HUMAN_BODY_ACCURATE
        /// </summary>
        HUMAN_BODY_ACCURATE_DETECTION,
        /// <summary>
        /// related to sl.DETECTION_MODEL.HUMAN_BODY_FAST
        /// </summary>
        HUMAN_BODY_38_FAST_DETECTION,
        /// <summary>
        /// related to sl.DETECTION_MODEL.HUMAN_BODY_MEDIUM
        /// </summary>
        HUMAN_BODY_38_MEDIUM_DETECTION,
        /// <summary>
        /// related to sl.DETECTION_MODEL.HUMAN_BODY_ACCURATE
        /// </summary>
        HUMAN_BODY_38_ACCURATE_DETECTION,
#if false
        /// <summary>
        /// related to sl.DETECTION_MODEL.HUMAN_BODY_FAST
        /// </summary>
        HUMAN_BODY_70_FAST_DETECTION,
        /// <summary>
        /// related to sl.DETECTION_MODEL.HUMAN_BODY_MEDIUM
        /// </summary>
        HUMAN_BODY_70_MEDIUM_DETECTION,
        /// <summary>
        /// related to sl.DETECTION_MODEL.HUMAN_BODY_ACCURATE
        /// </summary>
        HUMAN_BODY_70_ACCURATE_DETECTION,
#endif
        /// <summary>
        /// related to sl.DETECTION_MODEL.PERSON_HEAD
        /// </summary>
        PERSON_HEAD_DETECTION,
        /// <summary>
        /// related to sl.DETECTION_MODEL.PERSON_HEAD_ACCURATE
        /// </summary>
        PERSON_HEAD_ACCURATE_DETECTION,
        /// <summary>
        /// related to sl.BatchParameters.enable
        /// </summary>
        REID_ASSOCIATION,
        /// <summary>
        /// related to sl.DETECTION_MODEL.NEURAL
        /// </summary>
        NEURAL_DEPTH,

        LAST
    };

    /// <summary>
    /// Lists of supported bounding box preprocessing
    /// </summary>
    public enum OBJECT_FILTERING_MODE
    {
        /// <summary>
        /// SDK will not apply any preprocessing to the detected objects 
        /// </summary>
        NONE,
        /// <summary>
        /// SDK will remove objects that are in the same 3D position as an already tracked object (independant of class ID). Default value
        /// </summary>
        NMS3D,
        /// <summary>
        /// SDK will remove objects that are in the same 3D position as an already tracked object of the same class ID
        /// </summary>
        NMS3D_PER_CLASS
    };

    ///\ingroup Object_group
    /// <summary>
    /// semantic of human body parts and order keypoints for BODY_FORMAT.POSE_18.
    /// </summary>
    public enum BODY_18_PARTS
    {
        NOSE = 0,
        NECK = 1,
        RIGHT_SHOULDER = 2,
        RIGHT_ELBOW = 3,
        RIGHT_WRIST = 4,
        LEFT_SHOULDER = 5,
        LEFT_ELBOW = 6,
        LEFT_WRIST = 7,
        RIGHT_HIP = 8,
        RIGHT_KNEE = 9,
        RIGHT_ANKLE = 10,
        LEFT_HIP = 11,
        LEFT_KNEE = 12,
        LEFT_ANKLE = 13,
        RIGHT_EYE = 14,
        LEFT_EYE = 15,
        RIGHT_EAR = 16,
        LEFT_EAR = 17,
        LAST = 18
    };

    ///\ingroup Object_group
    /// <summary>
    /// ssemantic of human body parts and order keypoints for BODY_FORMAT.POSE_34.
    /// </summary>
    public enum BODY_34_PARTS
    {
        PELVIS = 0,
        NAVAL_SPINE = 1,
        CHEST_SPINE = 2,
        NECK = 3,
        LEFT_CLAVICLE = 4,
        LEFT_SHOULDER = 5,
        LEFT_ELBOW = 6,
        LEFT_WRIST = 7,
        LEFT_HAND = 8,
        LEFT_HANDTIP = 9,
        LEFT_THUMB = 10,
        RIGHT_CLAVICLE = 11,
        RIGHT_SHOULDER = 12,
        RIGHT_ELBOW = 13,
        RIGHT_WRIST = 14,
        RIGHT_HAND = 15,
        RIGHT_HANDTIP = 16,
        RIGHT_THUMB = 17,
        LEFT_HIP = 18,
        LEFT_KNEE = 19,
        LEFT_ANKLE = 20,
        LEFT_FOOT = 21,
        RIGHT_HIP = 22,
        RIGHT_KNEE = 23,
        RIGHT_ANKLE = 24,
        RIGHT_FOOT = 25,
        HEAD = 26,
        NOSE = 27,
        LEFT_EYE = 28,
        LEFT_EAR = 29,
        RIGHT_EYE = 30,
        RIGHT_EAR = 31,
        LEFT_HEEL = 32,
        RIGHT_HEEL = 33,
        LAST = 34
    };

    ///\ingroup Object_group
	/// <summary>
	/// semantic of human body parts and order keypoints for BODY_FORMAT.POSE_38.
	/// </summary>
	public enum BODY_38_PARTS 
    {
        PELVIS,
        SPINE_1,
        SPINE_2,
        SPINE_3,
        NECK,
        NOSE,
        LEFT_EYE,
        RIGHT_EYE,
        LEFT_EAR,
        RIGHT_EAR,
        LEFT_CLAVICLE,
        RIGHT_CLAVICLE,
        LEFT_SHOULDER,
        RIGHT_SHOULDER,
        LEFT_ELBOW,
        RIGHT_ELBOW,
        LEFT_WRIST,
        RIGHT_WRIST,
        LEFT_HIP,
        RIGHT_HIP,
        LEFT_KNEE,
        RIGHT_KNEE,
        LEFT_ANKLE,
        RIGHT_ANKLE,
        LEFT_BIG_TOE,
        RIGHT_BIG_TOE,
        LEFT_SMALL_TOE,
        RIGHT_SMALL_TOE,
        LEFT_HEEL,
        RIGHT_HEEL,
        // Hands
        LEFT_HAND_THUMB_4,
        RIGHT_HAND_THUMB_4,
        LEFT_HAND_INDEX_1,
        RIGHT_HAND_INDEX_1,
        LEFT_HAND_MIDDLE_4,
        RIGHT_HAND_MIDDLE_4,
        LEFT_HAND_PINKY_1,
        RIGHT_HAND_PINKY_1,
        LAST
    };

#if false
    ///\ingroup Object_group
    /// <summary>
    /// ssemantic of human body parts and order keypoints for BODY_FORMAT.POSE_70.
    /// </summary>
    public enum BODY_70_PARTS
    {
        PELVIS,
        SPINE_1,
        SPINE_2,
        SPINE_3,
        NECK,
        NOSE,
        LEFT_EYE,
        RIGHT_EYE,
        LEFT_EAR,
        RIGHT_EAR,
        LEFT_CLAVICLE,
        RIGHT_CLAVICLE,
        LEFT_SHOULDER,
        RIGHT_SHOULDER,
        LEFT_ELBOW,
        RIGHT_ELBOW,
        LEFT_WRIST,
        RIGHT_WRIST,
        LEFT_HIP,
        RIGHT_HIP,
        LEFT_KNEE,
        RIGHT_KNEE,
        LEFT_ANKLE,
        RIGHT_ANKLE,
        LEFT_BIG_TOE,
        RIGHT_BIG_TOE,
        LEFT_SMALL_TOE,
        RIGHT_SMALL_TOE,
        LEFT_HEEL,
        RIGHT_HEEL,
        // Hands
        // Left
        LEFT_HAND_THUMB_1,
        LEFT_HAND_THUMB_2,
        LEFT_HAND_THUMB_3,
        LEFT_HAND_THUMB_4,
        LEFT_HAND_INDEX_1,
        LEFT_HAND_INDEX_2,
        LEFT_HAND_INDEX_3,
        LEFT_HAND_INDEX_4,
        LEFT_HAND_MIDDLE_1,
        LEFT_HAND_MIDDLE_2,
        LEFT_HAND_MIDDLE_3,
        LEFT_HAND_MIDDLE_4,
        LEFT_HAND_RING_1,
        LEFT_HAND_RING_2,
        LEFT_HAND_RING_3,
        LEFT_HAND_RING_4,
        LEFT_HAND_PINKY_1,
        LEFT_HAND_PINKY_2,
        LEFT_HAND_PINKY_3,
        LEFT_HAND_PINKY_4,
        //Right
        RIGHT_HAND_THUMB_1,
        RIGHT_HAND_THUMB_2,
        RIGHT_HAND_THUMB_3,
        RIGHT_HAND_THUMB_4,
        RIGHT_HAND_INDEX_1,
        RIGHT_HAND_INDEX_2,
        RIGHT_HAND_INDEX_3,
        RIGHT_HAND_INDEX_4,
        RIGHT_HAND_MIDDLE_1,
        RIGHT_HAND_MIDDLE_2,
        RIGHT_HAND_MIDDLE_3,
        RIGHT_HAND_MIDDLE_4,
        RIGHT_HAND_RING_1,
        RIGHT_HAND_RING_2,
        RIGHT_HAND_RING_3,
        RIGHT_HAND_RING_4,
        RIGHT_HAND_PINKY_1,
        RIGHT_HAND_PINKY_2,
        RIGHT_HAND_PINKY_3,
        RIGHT_HAND_PINKY_4,
        LAST

    };

#endif
    ///\ingroup Object_group
    /// <summary>
    /// Contains batched data of a detected object
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public class ObjectsBatch
    {
        /// <summary>
        /// How many data were stored. Use this to iterate through the top of position/velocity/bounding_box/...; objects with indexes greater than numData are empty.
        /// </summary>
        public int numData = 0;
        /// <summary>
        /// The trajectory id
        /// </summary>
        public int id = 0;
        /// <summary>
        /// Object Category. Identity the object type
        /// </summary>
        public OBJECT_CLASS label = OBJECT_CLASS.LAST;
        /// <summary>
        /// Object subclass
        /// </summary>
        public OBJECT_SUBCLASS sublabel = OBJECT_SUBCLASS.LAST;
        /// <summary>
        ///  Defines the object tracking state
        /// </summary>
        public OBJECT_TRACKING_STATE trackingState = OBJECT_TRACKING_STATE.TERMINATE;
        /// <summary>
        /// A sample of 3d position
        /// </summary>
        public Vector3[] positions = new Vector3[(int)Constant.MAX_BATCH_SIZE];
        /// <summary>
        /// a sample of the associated position covariance
        /// </summary>
        public float[,] positionCovariances = new float[(int)Constant.MAX_BATCH_SIZE, 6];
        /// <summary>
        /// A sample of 3d velocity
        /// </summary>
        public Vector3[] velocities = new Vector3[(int)Constant.MAX_BATCH_SIZE];
        /// <summary>
        /// The associated position timestamp
        /// </summary>
        public ulong[] timestamps = new ulong[(int)Constant.MAX_BATCH_SIZE];
        /// <summary>
        /// A sample of 3d bounding boxes
        /// </summary>
        public Vector3[,] boundingBoxes = new Vector3[(int)Constant.MAX_BATCH_SIZE, 8];
        /// <summary>
        /// 2D bounding box of the person represented as four 2D points starting at the top left corner and rotation clockwise.
        /// Expressed in pixels on the original image resolution, [0, 0] is the top left corner.
        ///      A ------ B
        ///      | Object |
        ///      D ------ C
        /// </summary>
        public Vector2[,] boundingBoxes2D = new Vector2[(int)Constant.MAX_BATCH_SIZE, 4];
        /// <summary>
        /// a sample of object detection confidence
        /// </summary>
        public float[] confidences = new float[(int)Constant.MAX_BATCH_SIZE];
        /// <summary>
        /// a sample of the object action state
        /// </summary>
        public OBJECT_ACTION_STATE[] actionStates = new OBJECT_ACTION_STATE[(int)Constant.MAX_BATCH_SIZE];
        /// <summary>
        /// bounds the head with four 2D points.
        /// Expressed in pixels on the original image resolution.
        /// Not available with DETECTION_MODEL.MULTI_CLASS_BOX.
        /// </summary>
        public Vector2[,] headBoundingBoxes2D = new Vector2[(int)Constant.MAX_BATCH_SIZE, 8];
        /// <summary>
        /// bounds the head with eight 3D points.
		/// Defined in sl.InitParameters.UNIT, expressed in RuntimeParameters.measure3DReferenceFrame.
		/// Not available with DETECTION_MODEL.MULTI_CLASS_BOX.
        /// </summary>
        public Vector3[,] headBoundingBoxes = new Vector3[(int)Constant.MAX_BATCH_SIZE, 8];
        /// <summary>
        /// 3D head centroid.
		/// Defined in sl.InitParameters.UNIT, expressed in RuntimeParameters.measure3DReferenceFrame.
		/// Not available with DETECTION_MODEL.MULTI_CLASS_BOX.
        /// </summary>
        public Vector3[] headPositions = new Vector3[(int)Constant.MAX_BATCH_SIZE];
    }

#if false

    ///\ingroup Object_group
    /// <summary>
    /// Contains batched data of a detected object
    /// </summary>
    public class BodiesBatch
    {
        /// <summary>
        /// How many data were stored. Use this to iterate through the top of position/velocity/bounding_box/...; objects with indexes greater than numData are empty.
        /// </summary>
        public int numData = 0;
        /// <summary>
        /// The trajectory id
        /// </summary>
        public int id = 0;
        /// <summary>
        ///  Defines the object tracking state
        /// </summary>
        public OBJECT_TRACKING_STATE trackingState = OBJECT_TRACKING_STATE.TERMINATE;
        /// <summary>
        /// A sample of 3d position
        /// </summary>
        public Vector3[] positions = new Vector3[(int)Constant.MAX_BATCH_SIZE];
        /// <summary>
        /// a sample of the associated position covariance
        /// </summary>
        public float[,] positionCovariances = new float[(int)Constant.MAX_BATCH_SIZE, 6];
        /// <summary>
        /// A sample of 3d velocity
        /// </summary>
        public Vector3[] velocities = new Vector3[(int)Constant.MAX_BATCH_SIZE];
        /// <summary>
        /// The associated position timestamp
        /// </summary>
        public ulong[] timestamps = new ulong[(int)Constant.MAX_BATCH_SIZE];
        /// <summary>
        /// A sample of 3d bounding boxes
        /// </summary>
        public Vector3[,] boundingBoxes = new Vector3[(int)Constant.MAX_BATCH_SIZE, 8];
        /// <summary>
        /// 2D bounding box of the person represented as four 2D points starting at the top left corner and rotation clockwise.
        /// Expressed in pixels on the original image resolution, [0, 0] is the top left corner.
        ///      A ------ B
        ///      | Object |
        ///      D ------ C
        /// </summary>
        public Vector2[,] boundingBoxes2D = new Vector2[(int)Constant.MAX_BATCH_SIZE, 4];
        /// <summary>
        /// a sample of object detection confidence
        /// </summary>
        public float[] confidences = new float[(int)Constant.MAX_BATCH_SIZE];
        /// <summary>
        /// a sample of the object action state
        /// </summary>
        public OBJECT_ACTION_STATE[] actionStates = new OBJECT_ACTION_STATE[(int)Constant.MAX_BATCH_SIZE];
        /// <summary>
        /// keypoints 2D
        /// </summary>
        public Vector2[,] keypoints2D = new Vector2[(int)Constant.MAX_BATCH_SIZE, 70];
        /// <summary>
        /// keypoints
        /// </summary>
        public Vector3[,] keypoints = new Vector3[(int)Constant.MAX_BATCH_SIZE, 70];
        /// <summary>
        /// bounds the head with four 2D points.
        /// Expressed in pixels on the original image resolution.
        /// Not available with DETECTION_MODEL.MULTI_CLASS_BOX.
        /// </summary>
        public Vector2[,] headBoundingBoxes2D = new Vector2[(int)Constant.MAX_BATCH_SIZE, 8];
        /// <summary>
        /// bounds the head with eight 3D points.
		/// Defined in sl.InitParameters.UNIT, expressed in RuntimeParameters.measure3DReferenceFrame.
		/// Not available with DETECTION_MODEL.MULTI_CLASS_BOX.
        /// </summary>
        public Vector3[,] headBoundingBoxes = new Vector3[(int)Constant.MAX_BATCH_SIZE, 8];
        /// <summary>
        /// 3D head centroid.
		/// Defined in sl.InitParameters.UNIT, expressed in RuntimeParameters.measure3DReferenceFrame.
		/// Not available with DETECTION_MODEL.MULTI_CLASS_BOX.
        /// </summary>
        public Vector3[] headPositions = new Vector3[(int)Constant.MAX_BATCH_SIZE];
        /// <summary>
        /// a sample of the associated position covariance
        /// </summary>
        public float[,] keypointsConfidences = new float[(int)Constant.MAX_BATCH_SIZE, 70];
    }

#endif



    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////////
    ////////////////////////////////////////  Fusion API ///////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region Fusion API Module

    /// \ingroup Fusion_group
    /// <summary>
    /// Lists the types of error that can be raised by the Fusion.
    /// </summary>
    public enum FUSION_ERROR_CODE
    {
        /// <summary>
        /// Senders are using different body formats. Consider changing them.
        /// </summary>
        WRONG_BODY_FORMAT = -7,
        /// <summary>
        /// The following module was not enabled.
        /// </summary>
        NOT_ENABLE = -6,
        /// <summary>
        /// Some sources are provided by SVO and others by LIVE stream.
        /// </summary>
        INPUT_FEED_MISMATCH = -5,
        /// <summary>
        /// Connection timed out. Unable to reach the sender. Verify the sender's IP/port.
        /// </summary>
        CONNECTION_TIMED_OUT = -4,
        /// <summary>
        /// Intra-process shared memory allocation issue. Multiple connections to the same data.
        /// </summary>
        MEMORY_ALREADY_USED = -3,
        /// <summary>
        /// The provided IP address format is incorrect. Please provide the IP in the format 'a.b.c.d', where (a, b, c, d) are numbers between 0 and 255.
        /// </summary>
        BAD_IP_ADDRESS = -2,
        /// <summary>
        /// Standard code for unsuccessful behavior.
        /// </summary>
        FAILURE = -1,
        /// <summary>
        /// Standard code for successful behavior.
        /// </summary>
        SUCCESS = 0,
        /// <summary>
        /// Significant differences observed between sender's FPS.
        /// </summary>
        ERRATIC_FPS = 1,
        /// <summary>
        /// At least one sender has an FPS lower than 10 FPS.
        /// </summary>
        FPS_TOO_LOW = 2,
        /// <summary>
        /// Problem detected with ingested timestamp. Sample data will be ignored.
        /// </summary>
        INVALID_TIMESTAMP = 3,
        /// <summary>
        /// Problem detected with ingested covariance. Sample data will be ignored.
        /// </summary>
        INVALID_COVARIANCE = 4,
        /// <summary>
        /// All data from all sources has been consumed. No new data is available for processing.
        /// </summary>
        NO_NEW_DATA_AVAILABLE = 5
    }

    /// \ingroup Fusion_group
    /// <summary>
    /// Lists the types of error that can be raised during the Fusion by senders.
    /// </summary>
    public enum SENDER_ERROR_CODE
    {
        /// <summary>
        /// The sender has been disconnected.
        /// </summary>
        DISCONNECTED = -1,
        /// <summary>
        ///  Standard code for successful behavior.
        /// </summary>
        SUCCESS = 0,
        /// <summary>
        /// The sender encountered a grab error.
        /// </summary>
        GRAB_ERROR = 1,
        /// <summary>
        /// The sender does not run with a constant frame rate.
        /// </summary>
        ERRATIC_FPS = 2,
        /// <summary>
        /// The frame rate of the sender is lower than 10 FPS.
        /// </summary>
        FPS_TOO_LOW = 3
    }

    /// \ingroup Fusion_group
    /// <summary>
    /// Holds the options used to initialize the \ref Fusion object.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct InitFusionParameters
    {
        /// <summary>
        /// This parameter allows you to select the unit to be used for all metric values of the SDK (depth, point cloud, tracking, mesh, and others).
	    /// Default : \ref UNIT "UNIT::MILLIMETER"
        /// </summary>
        public UNIT coordinateUnits;

        /// <summary>
        /// Positional tracking, point clouds and many other features require a given \ref COORDINATE_SYSTEM to be used as reference.
        /// This parameter allows you to select the \ref COORDINATE_SYSTEM used by the \ref Camera to return its measures.
    	/// \n This defines the order and the direction of the axis of the coordinate system.
    	/// \n Default : \ref COORDINATE_SYSTEM "COORDINATE_SYSTEM::IMAGE"
        /// </summary>
        public COORDINATE_SYSTEM coordinateSystem;

        /// <summary>
        /// It allows users to extract some stats of the Fusion API like drop frame of each camera, latency, etc...
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool outputPerformanceMetrics;

        /// <summary>
        /// Enable the verbosity mode of the SDK.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool verbose;

        /// <summary>
        /// If specified change the number of period necessary for a source to go in timeout without data. For example, if you set this to 5
        /// then, if any source do not receive data during 5 period, these sources will go to timeout and will be ignored.	    /// if any source does not receive data during 5 periods, these sources will go into timeout and will be ignored.
        /// </summary>
        public uint timeoutPeriodsNumber;
    }

    /// \ingroup Fusion_group
    /// <summary>
    /// Lists the types of communications available for Fusion app.
    /// </summary>
    public enum COMM_TYPE
    {
        /// <summary>
        /// The sender and receiver are on the samed local network and communicate by RTP, communication can be affected by the network load.
        /// </summary>
        LOCAL_NETWORK, 
        /// <summary>
        /// Both sender and receiver are declared by the same process, can be in different threads, this communication is optimized.
        /// </summary>
        INTRA_PROCESS 
    };

    /// \ingroup Fusion_group
    /// <summary>
    /// Holds the communication parameter to configure the connection between senders and receiver
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CommunicationParameters
    {
        /// <summary>
        /// Type of communication
        /// </summary>
        public COMM_TYPE communicationType;
        /// <summary>
        /// The comm port used for streaming the data
        /// </summary>
	    uint ipPort;
        /// <summary>
        /// The IP address of the sender
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
        char[] ipAdd;
    };

    /// \ingroup Fusion_group
    /// <summary>
    /// Stores the Fusion configuration, can be read from /write to a Json file.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FusionConfiguration
    {
        /// <summary>
        /// The serial number of the used ZED camera.
        /// </summary>
        int serialnumber;
        /// <summary>
        /// The communication parameters to connect this camera to the Fusion.
        /// </summary>
        public CommunicationParameters commParam;
        /// <summary>
        /// The WORLD position of the camera for Fusion.
        /// </summary>
        public Vector3 position;
        /// <summary>
        /// The WORLD rotation of the camera for Fusion.
        /// </summary>
        public Quaternion rotation;
        /// <summary>
        /// The input type for the current camera.
        /// </summary>
        public INPUT_TYPE inputType;
    };


    /// \ingroup Fusion_group
    /// <summary>
    /// Holds the options used to initialize the body tracking module of the \ref Fusion.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct BodyTrackingFusionParameters
    {
        /// <summary>
        /// Defines if the object detection will track objects across images flow.
        ///
        /// Default: true
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool enableTracking;

        /// <summary>
        /// Defines if the body fitting will be applied.
        ///
        /// Default: false
        /// \note If you enable it and the camera provides data as BODY_18 the fused body format will be BODY_34.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool enableBodyFitting;
    }

    /// \ingroup Fusion_group
    /// <summary>
    /// Holds the options used to change the behavior of the body tracking module at runtime.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct BodyTrackingFusionRuntimeParameters
    {
        /// <summary>
        /// If the fused skeleton has less than skeleton_minimum_allowed_keypoints keypoints, it will be discarded.
        ///
        /// Default: -1.
        /// </summary>
        public int skeletonMinimumAllowedKeypoints;

        /// <summary>
        /// If a skeleton was detected in less than skeleton_minimum_allowed_camera cameras, it will be discarded.
        ///
        /// Default: -1.
        /// </summary>
        public int skeletonMinimumAllowedCameras;

        /// <summary>
	    /// This value controls the smoothing of the tracked or fitted fused skeleton.
        ///
        /// It is ranged from 0 (low smoothing) and 1 (high smoothing).
        /// \n Default: 0.
        /// </summary>
        public float skeletonSmoothing;
    }

    /// \ingroup Fusion_group
    /// <summary>
    /// Used to identify a specific camera in the Fusion API
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CameraIdentifier
    {
        /// <summary>
        /// Serial Number of the camera.
        /// </summary>
        public ulong sn;
    }

    /// \ingroup Fusion_group
    /// <summary>
    /// Holds the metrics of a sender in the fusion process.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct CameraMetrics
    {
        public CameraIdentifier uuid;

        /// <summary>
        /// FPS of the received data.
        /// </summary>
        public float receivedFps;

        /// <summary>
        /// Latency (in seconds) of the received data.
        /// </summary>
        public float receivedLatency;

        /// <summary>
        /// Latency (in seconds) after Fusion synchronization.
        /// </summary>
        public float syncedLatency;

        /// <summary>
        /// If no data present is set to false.
        /// </summary>
        [MarshalAs(UnmanagedType.U1)]
        public bool isPresent;

        /// <summary>
        /// Percent of detection par image during the last second in %, a low value means few detections occurs lately.
        /// </summary>
        public float ratioDetection;

        /// <summary>
        /// Average time difference for the current fused data.
        /// </summary>
        public float deltaTs;

    }

    /// \ingroup Fusion_group
    /// <summary>
    /// Holds the metrics of the fusion process.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct FusionMetrics
    {
        /// <summary>
        /// Mean number of camera that provides data during the past second.
        /// </summary>
        public float meanCameraFused;

        /// <summary>
        /// Standard deviation of the data timestamp fused, the lower the better.
        /// </summary>
        public float meanStdevBetweenCamera;

        /// <summary>
        /// Sender metrics.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)(Constant.MAX_FUSED_CAMERAS))]
        public CameraMetrics[] cameraIndividualStats;
    };

    #endregion

    ////////////////////////////////////////////////////////////////////////////////////////////////////////
    /////////////////////////////////////////  GNSS API ////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////

    #region GNSS API

    /// \ingroup Fusion_group
    /// <summary>
    /// Current state of GNSS fusion.
    /// </summary>
    public enum GNSS_CALIBRATION_STATE
    {
        /// <summary>
        /// The GNSS/VIO calibration has not been completed yet. Please continue moving the robot while ingesting GNSS data to perform the calibration.
        /// </summary>
        GNSS_CALIBRATION_STATE_NOT_CALIBRATED = 0,
        /// <summary>
        /// The GNSS/VIO calibration is completed.
        /// </summary>
        GNSS_CALIBRATION_STATE_CALIBRATED = 1,
        /// <summary>
        /// A GNSS/VIO re-calibration is in progress in the background. Current geo-tracking services may not be entirely accurate.
        /// </summary>
        GNSS_CALIBRATION_STATE_RE_CALIBRATION_IN_PROGRESS = 2
    };

    /// \ingroup Fusion_group
    /// <summary>
    /// Contains all GNSS data to be used for positional tracking as prior.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct GNSSData
    {
        /// <summary>
        /// Longitude in radian
        /// </summary>
        public double longitude;
        /// <summary>
        /// Latitude in radian
        /// </summary>
        public double latitude;
        /// <summary>
        /// Altitude in meter
        /// </summary>
        public double altitude;
        /// <summary>
        /// Timestamp of GNSS position, must be aligned with camera time reference.
        /// </summary>
        public ulong ts;
        /// <summary>
        /// Position covariance in meter must be expressed in ENU coordinate system.
	    /// For eph, epv GNSS sensors, set it as follow: {eph*eph, 0, 0, 0, eph*eph, 0, 0, 0, epv*epv}.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 9)]
        public double[] positionCovariance;
        /// <summary>
        /// Longitude standard deviation.
        /// </summary>
        public double longitudeStd;
        /// <summary>
        /// Latitude standard deviation.
        /// </summary>
        public double latitudeStd;
        /// <summary>
        /// Altitude standard deviation.
        /// </summary>
        public double altitudeStd;
    }

    /// \ingroup Fusion_group
    /// <summary>
    /// Holds Geo reference position.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct GeoPose
    {
        /// <summary>
        /// The translation defining the pose in ENU.
        /// </summary>
        public Vector3 translation;
        /// <summary>
        /// The rotation defining the pose in ENU.
        /// </summary>
        public Quaternion rotation;
        /// <summary>
        /// The pose covariance in ENU.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 36)]
        public float[] poseCovariance;
        /// <summary>
        /// The horizontal accuracy.
        /// </summary>
        public double horizontalAccuracy;
        /// <summary>
        /// The vertical accuracy.
        /// </summary>
        public double verticalAccuracy;
        /// <summary>
        /// The latitude, longitude, altitude.
        /// </summary>
        public LatLng latCoordinate;
        /// <summary>
        /// The heading.
        /// </summary>
        public double heading;
        /// <summary>
        /// The timestamp of GeoPose.
        /// </summary>
        public ulong timestamp;
    };

    /// \ingroup Fusion_group
    /// <summary>
    /// Represents a world position in ECEF format.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct ECEF
    {
        /// <summary>
        /// x coordinate of ECEF.
        /// </summary>
        public double x;
        /// <summary>
        /// y coordinate of ECEF.
        /// </summary>
        public double y;
        /// <summary>
        /// z coordinate of ECEF.
        /// </summary>
        public double z;
    }

    /// \ingroup Fusion_group
    /// <summary>
    /// Represents a world position in LatLng format.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct LatLng
    {
        /// <summary>
        /// Latitude in radian.
        /// </summary>
        public double latitude;
        /// <summary>
        /// Longitude in radian.
        /// </summary>
        public double longitude;
        /// <summary>
        /// Altitude in meter.
        /// </summary>
        public double altitude;
    }

    /// \ingroup Fusion_group
    /// <summary>
    /// Represents a world position in UTM format.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct UTM
    {
        /// <summary>
        /// Northing coordinate.
        /// </summary>
        public double northing;
        /// <summary>
        /// Easting coordinate.
        /// </summary>
        public double easting;
        /// <summary>
        /// Gamma coordinate.
        /// </summary>
        public double gamma;
        /// <summary>
        /// UTMZone of the coordinate.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string UTMZone;
    }

    /// \ingroup Fusion_group
    /// <summary>
    /// Holds the options used for calibrating GNSS / VIO.
    /// </summary>
    public class GNSSCalibrationParameters
    {
        /// <summary>
        /// This parameter defines the target yaw uncertainty at which the calibration process between GNSS and VIO concludes.
        /// The unit of this parameter is in radian.
        /// 
        /// Default: 0.1 radians
        /// </summary>
        public float targetYawUncertainty = 0.1f;
        /// <summary>
        /// When this parameter is enabled (set to true), the calibration process between GNSS and VIO accounts for the uncertainty in the determined translation, thereby facilitating the calibration termination. 
        /// The maximum allowable uncertainty is controlled by the 'target_translation_uncertainty' parameter.
	    ///
        /// Default: false
        /// </summary>
        public bool enableTranslationUncertaintyTarget = false;
        /// <summary>
        /// This parameter defines the target translation uncertainty at which the calibration process between GNSS and VIO concludes.
        ///
        /// Default: 10e-2 (10 centimeters)
        /// </summary>
        public float targetTranslationUncertainty = 10e-2f;
        /// <summary>
        /// This parameter determines whether reinitialization should be performed between GNSS and VIO fusion when a significant disparity is detected between GNSS data and the current fusion data.
        /// It becomes particularly crucial during prolonged GNSS signal loss scenarios.
        /// 
        /// Default: true
        /// </summary>
        public bool enableReinitialization = true;
        /// <summary>
        /// This parameter determines the threshold for GNSS/VIO reinitialization.
        /// If the fused position deviates beyond out of the region defined by the product of the GNSS covariance and the gnss_vio_reinit_threshold, a reinitialization will be triggered.
        /// 
        /// Default: 5
        /// </summary>
        public float gnssVioReinitThreshold = 5;
        /// <summary>
        /// If this parameter is set to true, the fusion algorithm will used a rough VIO / GNSS calibration at first and then refine it.
        /// This allow you to quickly get a fused position.
        ///
        ///  Default: true
        /// </summary>     
        public bool enableRollingCalibration = true;

        public GNSSCalibrationParameters(float targetYawUncertainty_ = 0.1f, bool enableTranslationUncertaintyTarget_ = false, float targetTranslationUncertainty_ = 0.01f,
            bool enableReinitialization_ = true, float gnssVioReinitThreshold_ = 5, bool enableRollingCalibration_ = true)
        {
            targetYawUncertainty = targetYawUncertainty_;
            enableTranslationUncertaintyTarget = enableTranslationUncertaintyTarget_;
            targetTranslationUncertainty = targetTranslationUncertainty_;
            enableReinitialization = enableReinitialization_;
            gnssVioReinitThreshold = gnssVioReinitThreshold_;
            enableRollingCalibration = enableRollingCalibration_;
        }
    };

    /// \ingroup Fusion_group
    /// <summary>
    /// Holds the options used for initializing the positional tracking fusion module.
    /// </summary>
    public class PositionalTrackingFusionParameters
    {
        /// <summary>
        /// This attribute is responsible for enabling or not GNSS positional tracking fusion.
        /// </summary>
        public bool enableGNSSFusion = false;
        /// <summary>
        /// Control the VIO / GNSS calibration process.
        /// </summary>
        public GNSSCalibrationParameters gnssCalibrationParameters;
        /// <summary>
        /// Constructor
        /// </summary>
        public PositionalTrackingFusionParameters()
        {
            enableGNSSFusion = false;
            gnssCalibrationParameters = new GNSSCalibrationParameters();
        }
    }

    #endregion
}// end namespace sl
