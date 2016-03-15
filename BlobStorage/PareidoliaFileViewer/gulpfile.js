/*
This file in the main entry point for defining Gulp tasks and using Gulp plugins.
Click here to learn more. http://go.microsoft.com/fwlink/?LinkId=518007
*/

var gulp = require('gulp');
var browserify = require('gulp-browserify');
var plumber = require('gulp-plumber');
var concat = require('gulp-concat');

var util = require('gulp-util');
var minifyCss = require('gulp-minify-css');
var minifyHTML = require('gulp-minify-html');

var build = './client/build/';
var codeRoot = './client/public/';
var codeApp = codeRoot + 'app/';
var clientApp = build + 'app/';

var codeAppFile = codeApp + 'index.js',
    codeAppHtml = codeApp + 'index.html',
    cssDest = clientApp + 'stylesheets'
    viewsDest = clientApp + 'views';

gulp.task('styles', [], function () {
    return gulp.src(['./client/public/app/stylesheets/*'])
        .pipe(minifyCss())
        .pipe(gulp.dest(cssDest));
});

gulp.task('views', [], function () {
    "use strict";
    return gulp.src(['!./client/public/app/*', './client/public/app/**/*.html'])
        //.pipe(minifyHTML())
        .pipe(gulp.dest(viewsDest));
});

gulp.task('default', ['views', 'styles'], function () {
    'use strict';
    return gulp.src([codeAppFile])
        .pipe(plumber())
        .pipe(browserify({
            insertGlobals: true,
            debug: true
        }))
        // Bundle to a single file
        .pipe(concat('bundle.js'))
        /*jshint camelcase: false */
        //.pipe(gulpif(ugly(), uglify({compress:{drop_console:true}})))
        // Output it to our dist folder
        .pipe(gulp.dest(clientApp));
});