//using AVFoundation;
//using AuthenticationServices;
using prjGuIA.Models;
using System.Collections.ObjectModel;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision;
using Microsoft.Azure.CognitiveServices.Vision.ComputerVision.Models;
using Microsoft.Maui.Controls;
using System;
using System.IO;
using Firebase.Auth;
using Firebase.Auth.Providers;
using Firebase.Storage;
//using Android.App;
//using CoreImage;

namespace prjGuIA.Views;

public partial class PerfilInicial : ContentPage
{
    string authDomain = "fir-939dd.firebaseapp.com";
    string apiKey = "AIzaSyALbr1m_k3C3Q9Ssw9e9sZHBPu_qsKsjrg";
    string email = "user@gmail.com";
    string password = "tumama123";
    string token = string.Empty;
    string rutaStorage = "fir-939dd.appspot.com";
    public ObservableCollection<Perfiles> Perfiles { get; set; }
	public ObservableCollection<GenerarImagen> GenerarImagenes { get; set; }
	public PerfilInicial()
	{
		InitializeComponent();
		LoadData();
		BindingContext = this;
        MainThread.BeginInvokeOnMainThread(new Action(async() => await obtenerToken()));
    } 

    private void LoadData()
    {
        Perfiles = new ObservableCollection<Perfiles>
        {
            new Perfiles
            {
                Name = "Hector",
                ProfileImage = "imgPerfil1.jpg",
                NoPhotos = 12
            },
            new Perfiles
            {
                Name = "Martha",
                ProfileImage = "imgPerfil2.jpg",
                NoPhotos = 6
            },
            new Perfiles
            {
                Name = "Luis",
                ProfileImage = "imgPerfil3.jpg",
                NoPhotos = 4
            },
            new Perfiles
            {
                Name = "Joaquin",
                ProfileImage = "imgPerfil4.jpg",
                NoPhotos = 9
            }
        };
        GenerarImagenes = new ObservableCollection<GenerarImagen>
        {
            new GenerarImagen
            {
                ImagePath = "imgsitio1.jpge",
                MainKeyword = "Chichen Itza",
                Keywords = new List<string>
                {
                    "Historia, Mayas, Piramide, Maravilla del mundo"
                }
            },
            new GenerarImagen
            {
                ImagePath = "imgsitio2.jpge",
                MainKeyword = "Bellas Artes",
                Keywords = new List<string>
                {
                    "Muralismo, CDMX, Cultura, Vital"
                }
            },
            new GenerarImagen
            {
                ImagePath = "imgsitio3.jpge",
                MainKeyword = "Los cabos",
                Keywords = new List<string>
                {
                    "Naturaleza, Playas, BCS, Turismo"
                }
            },
            new GenerarImagen
            {
                ImagePath = "imgsitio4.jpge",
                MainKeyword = "Mural Diego Rivera",
                Keywords = new List<string>
                {
                    "Historia, Bellas Artes, Muralismo, Arte, Diego Rivera"
                }
            },
            new GenerarImagen
            {
                ImagePath = "imgsitio5.jpges",
                MainKeyword = "Calendario Azteca",
                Keywords = new List<string>
                {
                    "Historia, Aztecas, Vestigio , Calendario, Astrología"
                }
            },
        };
    }


    private async void btnCaptura(object sender, EventArgs e)
    {
        string localFilePath = "";
        if (MediaPicker.Default.IsCaptureSupported)
        {
            FileResult photo = await MediaPicker.Default.CapturePhotoAsync();

            if (photo != null)
            {
                // save the file into local storage
                localFilePath = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);
                using Stream sourceStream = await photo.OpenReadAsync();
                using FileStream localFileStream = File.OpenWrite(localFilePath);
                await sourceStream.CopyToAsync(localFileStream);
                var task = new FirebaseStorage(
                    rutaStorage,
                    new FirebaseStorageOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(token),
                        ThrowOnCancel = true
                    }
                )
                .Child("imagenes")
                .Child(photo.FileName)
                .PutAsync(await photo.OpenReadAsync());
                await analizarAsync(await task);
            }
            //await analizarAsync(photo.ToString());
        }
        //await Navigation.PushAsync(new ImagenVista());
    }

    private async Task analizarAsync(string image)
    {
        string foto = string.Empty;
        string key = "6915b090dfa248cfb887b6be73242553";
        string endpoint = "https://sacbe.cognitiveservices.azure.com/";
        ApiKeyServiceClientCredentials visionCredentials = new(key);
        ComputerVisionClient vision = new ComputerVisionClient(visionCredentials);
        vision.Endpoint = endpoint;

        // Read an image.
        List<VisualFeatureTypes?> features = new()
        {
            VisualFeatureTypes.Categories,
            VisualFeatureTypes.Description,
            VisualFeatureTypes.ImageType,
            VisualFeatureTypes.Tags,
            VisualFeatureTypes.Objects,
            VisualFeatureTypes.Adult,
            VisualFeatureTypes.Color,
            VisualFeatureTypes.Faces,
            VisualFeatureTypes.Brands,
            VisualFeatureTypes.ImageType
        };
        Console.WriteLine(" ");
        Console.WriteLine(" ");
        Console.WriteLine(" ");
        //ImageAnalysis analysis = await vision.AnalyzeImageAsync("https://www.yucatan.gob.mx/docs/galerias/chichen_itza/1.jpg", features, language: "es");
        ImageAnalysis analysis = await vision.AnalyzeImageAsync(image, features, language: "es");
        ImageDescriptionDetails description = analysis.Description;
        Console.WriteLine("aaaaaasadfasda" + description.Captions);
        Console.WriteLine(" ");
        Console.WriteLine(" ");
        Console.WriteLine(" ");
        string descrip = string.Empty;
        foreach (ImageTag tagg in analysis.Tags)
        {
            foto += tagg.Name;
            foto += ", ";
        }
        Console.WriteLine(foto); 
        foreach (var cap in analysis.Description.Captions)
        {
            descrip = cap.Text;
        }
        ImagenAnalisis datos = new ImagenAnalisis
        {
            Descripcion = descrip,
            Etiquetas = foto,
            ImgUrl = image
        };
        await Navigation.PushAsync(new ImagenVista
        {
            BindingContext = datos
        });
    }

    private async void btnRA(object sender, EventArgs e)
    {    
        await Launcher.Default.OpenAsync(new Uri("sacbe://"));
    }
    private async Task obtenerToken()
    {
        var client = new FirebaseAuthClient(new FirebaseAuthConfig()
        {
            ApiKey = apiKey,
            AuthDomain = authDomain,
            Providers = new FirebaseAuthProvider[]
            {
                new EmailProvider()
            }
        });
        var credenciales = await client.SignInWithEmailAndPasswordAsync(email, password);
        token = await credenciales.User.GetIdTokenAsync();
    }
}