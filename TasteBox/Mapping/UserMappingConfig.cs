namespace TasteBox.Mapping;

public class UserMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Register Request -> ApplicationUser
        config.NewConfig<RegisterRequest, ApplicationUser>()
            .Map(dest => dest.UserName, src => src.Email);

        // User + Roles -> UserResponse
        config.NewConfig<(ApplicationUser user, IList<string> roles), UserResponse>()
            .Map(dest => dest, src => src.user)
            .Map(dest => dest.Roles, src => src.roles);

        // CreateUserRequest -> ApplicationUser
        config.NewConfig<CreateUserRequest, ApplicationUser>()
            .Map(dest => dest.UserName, src => src.Email)
            .Map(dest => dest.EmailConfirmed, src => true);

        // UpdateUserRequest -> ApplicationUser
        config.NewConfig<UpdateUserRequest, ApplicationUser>()
            .Map(dest => dest.UserName, src => src.Email)
            .Map(dest => dest.NormalizedUserName, src => src.Email.ToUpper());

        // UpdateProfileRequest -> ApplicationUser (ignore null values)
        config.NewConfig<UpdateProfileRequest, ApplicationUser>()
            .IgnoreNullValues(true)
            .Map(dest => dest.NormalizedUserName, 
                src => !string.IsNullOrWhiteSpace(src.UserName) ? src.UserName.ToUpper() : null);

        // ApplicationUser -> UserProfileResponse
        config.NewConfig<ApplicationUser, UserProfileResponse>()
            .Map(dest => dest.FullName, src => $"{src.FirstName} {src.LastName}");
    }
}