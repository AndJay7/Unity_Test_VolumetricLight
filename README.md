# Unity_Test_VolumetricLight
Non-RayMarching Volume Light implementation

# Idea
General approach for creating Volume Light is using RayMarching methods to calculate intensity of light in defined point. This approach is computing heavy and generally can't be used on weaker hardware. Assuming a known shape of light, my idea is to use analitic/geometric approach to calculate approximate intensity of light in area.

# Cons
- partial penetration of light via opaque objects. This method can't take care of situation when objects partially cover light source.
- no support for inconsistent density
  
# Pros
- a lot of lighter then volumetric calculation
- can be added to scene without real light source. Volume light is faked by using mesh with specific shader

# Requirments
- URP
- Active DepthTexture 

# Current features
- Support for Spot/Cone light shape
- SRP Batcher compability
- VolumeLight component can be controlled by Light component or independently
- URP fog support

# Known issues
- halo effect on edge of Cone under some angles
- Volume mesh can disappear, when camera is close. It is because MeshRenderer bounding box is don't taking care of Light range

# To Do
- additional volume shapes (sphere, box)
- rewrite shader from ShaderGraph to classic shader
- GPU Instancing support
- better VolumeLight component Inspector
- VolumeLight component optimization
- hide MeshRenderer
