var express = require('express');
var router = express.Router();
var app = new express();
var mysql = require('mysql'); //load mysql module
var connection = mysql.createConnection({
    host: 'localhost',
    port: 3306,
    user: 'root',
    password: '!!tlstmdduf5',
    database: 'im'
});
var bodyParser = require('body-parser');

app.use(bodyParser.json());




connection.connect(function (err) {
    if (err) {
        console.error('mysql connection error');
        console.error(err);
        throw err;

    }
    router.post('/near/:filetype', function (req, res, next) {
        var find_id = req.body.id;  // to use finding user. 
       
        if (req.params.filetype == "json") {
            var query = connection.query('SELECT * FROM member WHERE id = ? ', find_id, function (err, rows) {
                res.json({ nickname: rows[0].nickname, status_message: rows[0].status_message })
            });
        }// client connect to get the json data type.
        else if (req.params.filetype == "image") {
            var query = connection.query('SELECT profile_picture FROM member WHERE id = ? ', find_id, function (err, rows) {
                res.sendfile(rows[0].profile_picture);
            });
        }// client connect to get the image data type.
        
    }); // this post is to view information for near user.

    router.post('/likeable/:filetype', function (req, res, next) {
        var find_id = req.body.id; // to use finding user.

        if (req.params.filetype == "json") {
            var query0 = connection.query('SELECT * FROM member WHERE id = ?', find_id, function (err, user) {
                var user_nickname = user[0].nickname;
                var user_phonenumber = user[0].phone_number;
                var user_statusmessage = user[0].status_message;
                
                var query1 = connection.query('SELECT link FROM link_rel WHERE id = ?', find_id, function (err, links) {
                    var link_arr = new Array(links.length);
                    for(var i=0; i<links.length; i++)
                    {
                        link_arr[i] = links[i].link;
                    }
                    res.json({ nickname: user_nickname, phone_number: user_phonenumber, status_message: user_statusmessage, link:link_arr });
                });
            });  
        } //client connect to get the json data type.

        else if (req.params.filetype == "image") {
            var query = connection.query('SELECT image_dir FROM image_rel WHERE id = ? ', find_id, function (err, images) {
                var images_dir = new Array(images.length);
                for (var i = 0; i < images.length; i++) {
                    images_dir[i] = images[i].image_dir;
                    res.sendfile(images_dir[i]);
                }
               

            });
        }

  
    }); // this post is to view information for likeable user.

    router.get('/', function (req, res, next) {
        console.log(req.ip);
  
        res.render('index1');

    });


});

module.exports = router;
