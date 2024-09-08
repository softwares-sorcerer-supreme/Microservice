namespace Shared.Constants;

public readonly struct ResponseErrorMessageCode
{
    //Auth
    //User Not Found
    public const string ERR_AUTH_0001 = nameof(ERR_AUTH_0001);
    
    //Invalid Email or Password
    public const string ERR_AUTH_0002 = nameof(ERR_AUTH_0002);
    
    //Email Already Exists
    public const string ERR_AUTH_0003 = nameof(ERR_AUTH_0003);
    
    //Token Invalid or Expired
    public const string ERR_AUTH_0004 = nameof(ERR_AUTH_0004);
    
    //Email Not Verified
    public const string ERR_AUTH_0005 = nameof(ERR_AUTH_0005);
    
    //Confirm Password Does Not Match
    public const string ERR_AUTH_0006 = nameof(ERR_AUTH_0006);
    
    //User Role Not Found
    public const string ERR_AUTH_0007 = nameof(ERR_AUTH_0007);
    
    //Role Not Found
    public const string ERR_AUTH_0008 = nameof(ERR_AUTH_0008);

    
    //Cart
    //Cart Not Found
    public const string ERR_CART_0001 = nameof(ERR_CART_0001);
    
    //Cart Item Not Found
    public const string ERR_CART_0002 = nameof(ERR_CART_0002);
    
    //Product
    //Product Not Found
    public const string ERR_PRODUCT_0001 = nameof(ERR_PRODUCT_0001);
    
    //Invalid Quantity
    public const string ERR_PRODUCT_0002 = nameof(ERR_PRODUCT_0002);
    
    //Product Already Exists
    public const string ERR_PRODUCT_0003 = nameof(ERR_PRODUCT_0003);
    
    //Common
    
    //Internal Server Error
    public const string ERR_SYS_0001 = nameof(ERR_SYS_0001);
    
    //Invalid Request
    public const string ERR_SYS_0002 = nameof(ERR_SYS_0002);
    
    //Not Found
    public const string ERR_SYS_0003 = nameof(ERR_SYS_0003);
}