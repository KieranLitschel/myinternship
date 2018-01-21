var Base64 = {
  characters: "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=" ,

  encode: function( string )
  {
      var characters = Base64.characters;
      var result     = '';

      var i = 0;
      do {
          var a = string.charCodeAt(i++);
          var b = string.charCodeAt(i++);
          var c = string.charCodeAt(i++);

          a = a ? a : 0;
          b = b ? b : 0;
          c = c ? c : 0;

          var b1 = ( a >> 2 ) & 0x3F;
          var b2 = ( ( a & 0x3 ) << 4 ) | ( ( b >> 4 ) & 0xF );
          var b3 = ( ( b & 0xF ) << 2 ) | ( ( c >> 6 ) & 0x3 );
          var b4 = c & 0x3F;

          if( ! b ) {
              b3 = b4 = 64;
          } else if( ! c ) {
              b4 = 64;
          }

          result += Base64.characters.charAt( b1 ) + Base64.characters.charAt( b2 ) + Base64.characters.charAt( b3 ) + Base64.characters.charAt( b4 );

      } while ( i < string.length );

      return result;
  } ,

  decode: function( string )
  {
      var characters = Base64.characters;
      var result     = '';

      var i = 0;
      do {
          var b1 = Base64.characters.indexOf( string.charAt(i++) );
          var b2 = Base64.characters.indexOf( string.charAt(i++) );
          var b3 = Base64.characters.indexOf( string.charAt(i++) );
          var b4 = Base64.characters.indexOf( string.charAt(i++) );

          var a = ( ( b1 & 0x3F ) << 2 ) | ( ( b2 >> 4 ) & 0x3 );
          var b = ( ( b2 & 0xF  ) << 4 ) | ( ( b3 >> 2 ) & 0xF );
          var c = ( ( b3 & 0x3  ) << 6 ) | ( b4 & 0x3F );

          result += String.fromCharCode(a) + (b?String.fromCharCode(b):'') + (c?String.fromCharCode(c):'');

      } while( i < string.length );

      return result;
  }
};
// Client ID and API key from the Developer Console
var CLIENT_ID = '482035422772-sl3mc5m1j55j62jr6c0njks77g7lf26c.apps.googleusercontent.com';
var test = '[{  "company_name": "JP Morgan",  "status"      : "Offer made"  },{  "company_name": "Google",  "status"      : "Telephone interview"  },{  "company_name" : "Amazon",  "status"       : "Apptitude Test"  },{  "compnay_name" : "TechCorp",  "status"       : "Rejected"}]';

// Array of API discovery doc URLs for APIs used by the quickstart
var DISCOVERY_DOCS = ["https://www.googleapis.com/discovery/v1/apis/gmail/v1/rest"];

// Authorization scopes required by the API; multiple scopes can be
// included, separated by spaces.
var SCOPES = 'https://www.googleapis.com/auth/gmail.readonly';
//alert("running");
var authorizeButton = document.getElementById('authorize-button');
//alert(authorizeButton);
var signoutButton = document.getElementById('signout-button');

/**
 *  On ad, called to load the auth2 library and API client library.
 */


/**
 *  Initializes the API client library and sets up sign-in state
 *  listeners.
 */
 function handleClientLoad() {
       gapi.load('auth2', function() {
         auth2 = gapi.auth2.init({
           client_id: '482035422772-sl3mc5m1j55j62jr6c0njks77g7lf26c.apps.googleusercontent.com',
           // Scopes to request in addition to 'profile' and 'email'
           //scope: 'additional_scope'
         });
       });
     }


/**
 *  Called when the signed in status changes, to update the UI
 *  appropriately. After a sign-in, the API is called.
 */
 $('#signinButton').click(function() {
   alert("firing!");
    // signInCallback defined in step 6.
    auth2.grantOfflineAccess().then(signInCallback);
  });

  function signInCallback(authResult) {
    if (authResult['code']) {
      console.log(authResult['code']);
      // Hide the sign-in button now that the user is authorized, for example:
      $('#signinButton').attr('style', 'display: none');

      // Send the code to the server
      $.get('https://localhost:5000/getStatus/'+authResult,function(data,status) {
        displayData(data);
      });
    }
  }

/**
 *  Sign in the user upon button click.
 */
function handleAuthClick(event) {
  gapi.auth2.getAuthInstance().signIn();
}

/**
 *  Sign out the user upon button click.
 */
function handleSignoutClick(event) {
  gapi.auth2.getAuthInstance().signOut();
}

/**
 * Append a pre element to the body containing the given message
 * as its text node. Used to display the results of the API call.
 *
 * @param {string} message Text to be placed in pre element.
 */
function appendPre(message) {
  var pre = document.getElementById('content');
  var textContent = document.createTextNode(message + '\n');
  pre.appendChild(textContent);
}

/**
 * Print all Labels in the authorized user's inbox. If no labels
 * are found an appropriate message is printed.
 */
 $('#signinButton').click(function() {
   // signInCallback defined in step 6.
   auth2.grantOfflineAccess().then(signInCallback);
 });

 function afterResp(resp) {
   var decoded = Base64.decode(resp.payload.parts[0].body.data);
   console.log(decoded);
   console.log(searchBody(decoded));


 }
 function getMessage(messageId) {
    var request = gapi.client.gmail.users.messages.get({
      'userId': 'me',
      'id': messageId
    });
    request.execute(function(response) {
      resp = response;
      afterResp(resp);
    });
}
function displayData(data) {
  var t = document.createElement('table'), tr, td, row, cell;
  var table = document.createElement('tbody');
  t.id = "content_table";
  tr = document.createElement('tr');
  td = document.createElement('td');
  td.id = "content_table";
  tr.appendChild(td);
  td.innerHTML = "Company"
  td = document.createElement('td');
  td.id = "content_table_header";
  tr.appendChild(td);
  td.innerHTML = "Status"
  tr.id = "content_table_header";
  table.appendChild(tr);

  for (i in data) {
    tr = document.createElement('tr');
    td = document.createElement('td');
    tr.id = "content_table";
    td.id = "content_table";
    tr.appendChild(td);
    td.innerHTML = data[i].company_name;
    td = document.createElement('td');
    tr.appendChild(td);
    td.id = "content_table";
    td.innerHTML = data[i].status;
    table.appendChild(tr);
    }
    t.appendChild(table);
  document.getElementsByTagName('body')[0].appendChild(t);
}
function sendUserID() {
  $.get("http://localhost:5000/API/" + s, function(data,status) {
    //empolyer,
    var obj = JSON.parse(data);
    for (x in obj) {
      console.log()
    }
  });
}
displayData(JSON.parse(test));
