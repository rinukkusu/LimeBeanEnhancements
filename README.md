# LimeBeanEnhancements

## Usage example
```cs
[BeanTable("user")]
public class User : LimeBeanEnhancements.BaseBean<User>
{
  public User(IBeanAPI beanApi) : base(beanApi)
  {
  }

  [BeanProperty("name")]
  public string Name { get; set; }

  [BeanProperty("ancestor")]
  [BeanRelation(typeof(User))]
  public User Ancestor { get; set; }
}
```
