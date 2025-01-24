According to Microsoft document, webview2 (async) methods and event should be run on UI thread.

Here is a sample to check use synchronous methods wrapping the async ones, thus you can forget the UI things.
