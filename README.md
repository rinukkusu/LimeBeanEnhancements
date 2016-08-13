# LimeBeanEnhancements

## Subclassing
```cs
[BeanTable("user")]
public class User : EnhancedBean<User>
{
  [BeanProperty("name")]
  public string Name { get; set; }

  [BeanProperty("ancestor")]
  [BeanRelation(typeof(User))]
  public User Ancestor { get; set; }
}
```

## Usage
```cs
// Getting a new instance of the User model
User user = _beanApi.Dispense<User>();
user.Name = "some name";
ulong id = (ulong)_beanApi.Store(user);

// Loading an instance into a User model
user = _beanApi.Load<User>(id);
user.Name = "new name";
_beanApi.Store(user);
```
