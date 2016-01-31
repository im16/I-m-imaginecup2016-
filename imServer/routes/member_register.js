var express = require('express');
var multer = require('multer');
var router = express.Router();
var mysql = require('mysql'); //load mysql module
var shell = require('shelljs'); //load shell to use full path of file 
var connection = mysql.createConnection({
    host: 'localhost',
    port: 3306,
    user: 'root',
    password: '!!tlstmdduf5',
    database: 'im'
});


var storage = multer.diskStorage({
    destination: function (req, file, cb) {
        var dir = './uploads/' + req.params.id + '/image';
        shell.mkdir('-p', dir);
        cb(null, dir);
    },
    filename: function (req, file, cb) {
       
        cb(null, 'profile.jpg');
    }
})

var upload = multer({ storage: storage });
connection.connect(function (err) {
    if (err) {
        console.error('mysql connection error');
        console.error(err);
        throw err;

    }

    router.post('/:id', upload.single('upl'),function (req, res) {
        console.log(req.body);
        console.log(req.file); //form files
        res.status(204).end();
    });



    router.get('/', function (req, res, next) {
        console.log(req.ip);
        res.render('index');
    });
    
    
});

module.exports = router;
