// This file can be replaced during build by using the `fileReplacements` array.
// `ng build --prod` replaces `environment.ts` with `environment.prod.ts`.
// The list of file replacements can be found in `angular.json`.
// For production use https://i395793core.venus.fhict.nl
// http://localhost:5000

export const environment = {
    production: true,
    hmr       : false,
    url: 'http://localhost:5000',
    firebase : {
        apiKey: "AIzaSyCe5Ue3tjyQB24WkSZuw7UUkdtbwyKes_Q",
        authDomain: "kidonapp-93b5a.firebaseapp.com",
        projectId: "kidonapp-93b5a",
        storageBucket: "kidonapp-93b5a.appspot.com",
        messagingSenderId: "1025392106185",
        appId: "1:1025392106185:web:ae22a0ef68c981122a8b18",
        measurementId: "G-8VJBQNY2Y3"
      }
};

/*
 * For easier debugging in development mode, you can import the following file
 * to ignore zone related error stack frames such as `zone.run`, `zoneDelegate.invokeTask`.
 *
 * This import should be commented out in production mode because it will have a negative impact
 * on performance if an error is thrown.
 */
// import 'zone.js/dist/zone-error';  // Included with Angular CLI.
