1. Can't build Xamarin.App with error saying :

`AppResources` does not contain a definition for `SomeString`
![image.png](/.attachments/image-775c62fd-b63c-4a12-a027-6b5e40401152.png)
- Solution: Open one of `AppResources.resx` and edit some file in it and save again. It will generating correct `AppResources.Designer.cs` and the error gone.