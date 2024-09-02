# 🎮 Unity-Restbase a Unity Firebase REST API Wrapper

Welcome to **Unity Restbase**! 🚀

Are you working on a Unity project that targets WebGL and noticed that the official Firebase SDK doesn’t play well with it? Or maybe you just want more control over your Firebase Firestore and Storage interactions in Unity? Well, look no further! (Hopefully...)

This project is a **lightweight** and **flexible** API wrapper that mimics the official Firebase Firestore and Storage APIs but under the hood, it uses simple REST API calls. The result? A smoother experience for Unity WebGL builds, **no more headaches**, and all the familiar methods and classes you know and love!

## 🌟 Why Use This?

- **Seamless Integration:** This API mirrors the official Firestore and Storage API structure, so you can easily swap out the original SDK with this without a massive code refactor. It’s almost like magic! 🎩✨
- **WebGL Ready:** Specifically designed to be compatible with Unity WebGL builds, while also working perfectly within the Unity editor.
- **Minimal Changes:** Keep your code clean! Replace the original API with ours in just a few steps, and you’re good to go.
- **All the Good Stuff:** Enjoy Firestore and Storage functionality with minimal overhead and without sacrificing your build compatibility.

## 🔧 How to Get Started

1. **Installation:** Clone this repository and add it to your Unity project. Add to Assets/Resources your google-services.json firebase config file.
2. **Usage:** Simply replace your Firebase Firestore and Storage API calls with our wrapper’s namespace, classes, and methods. You’re ready to go!
3. **Enjoy:** Sit back and watch as your WebGL builds interact seamlessly with Firebase Firestore and Storage! 🎉

## 🛠️ Features

- Full support for Firestore document operations
- Support for Firebase Storage operations (upload, download, delete)
- Authentication via Firebase Auth REST API
- Asynchronous operations with Unity’s `async` and `await` patterns
- Lightweight and easy to extend

## 📝 To-Do List

- [ ] **Real-time Listeners:** Add support for real-time Firestore listeners to automatically handle data changes.
- [ ] **Error Handling:** Improve and expand error handling across all API calls.
- [ ] **Extended Storage Features:** Implement more advanced features for Firebase Storage, like resumable uploads.
- [ ] **Real-time database:** support for real-time database.
- [ ] **Detailed Documentation:** Expand documentation with more use cases and advanced scenarios.

## 📚 Documentation

Coming soon?

## 👥 Contributing

Got an idea to make this even better? Found a bug? Contributions are welcome! Feel free to submit a pull request or open an issue.

## 📝 License

This project is licensed under the MIT License – see the [LICENSE](./LICENSE) file for details.

---

Give your Unity WebGL projects the Firebase love they deserve, without the hassle. Happy coding! 🎮🔥

