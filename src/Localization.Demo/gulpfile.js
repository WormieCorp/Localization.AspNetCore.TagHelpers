/// <binding AfterBuild='default' Clean='clean' ProjectOpened='watch' />
"use strict";

var gulp = require("gulp"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    htmlmin = require("gulp-htmlmin"),
    uglify = require("gulp-uglify"),
    merge = require("merge-stream"),
    del = require("del"),
    bundleconfig = require("./bundleconfig.json"); // make sure bundleconfig.json doesn't contain any comments

gulp.task("min", ["min:js", "min:css", "min:html"]);

var libFiles = {
  css: [
    "./node_modules/bootstrap/dist/css/bootstrap{.min,}.css"
  ],
  js: [
    "./node_modules/bootstrap/dist/js/bootstrap*",
    "./node_modules/jquery/dist/jquery{.min,}.js",
    "./node_modules/jquery-validation/dist/*.js",
    "./node_modules/jquery-validation-unobtrusive/*.js"
  ]
};

gulp.task("copy:js", function() {
  return gulp.src(libFiles.js)
    .pipe(gulp.dest("./wwwroot/lib/js"));
});

gulp.task("copy:css", function() {
  return gulp.src(libFiles.css)
    .pipe(gulp.dest("./wwwroot/lib/css"));
});

gulp.task("min:js", ["copy:js"], function () {
  var tasks = getBundles(".js").map(function (bundle) {
    return gulp.src(bundle.inputFiles, { base: "." })
			.pipe(concat(bundle.outputFileName))
			.pipe(uglify())
			.pipe(gulp.dest("."));
  });
  return merge(tasks);
});

gulp.task("min:css", ["copy:css"], function () {
  var tasks = getBundles(".css").map(function (bundle) {
    return gulp.src(bundle.inputFiles, { base: "." })
			.pipe(concat(bundle.outputFileName))
			.pipe(cssmin())
			.pipe(gulp.dest("."));
  });
  return merge(tasks);
});

gulp.task("min:html", function () {
  var tasks = getBundles(".html").map(function (bundle) {
    return gulp.src(bundle.inputFiles, { base: "." })
			.pipe(concat(bundle.outputFileName))
			.pipe(htmlmin({ collapseWhitespace: true, minifyCSS: true, minifyJS: true }))
			.pipe(gulp.dest("."));
  });
  return merge(tasks);
});

gulp.task("clean", function () {
  var files = bundleconfig.map(function (bundle) {
    return bundle.outputFileName;
  });

  return del(files);
});

gulp.task("watch", function () {
  getBundles(".js").forEach(function (bundle) {
    gulp.watch(bundle.inputFiles, ["min:js"]);
  });

  getBundles(".css").forEach(function (bundle) {
    gulp.watch(bundle.inputFiles, ["min:css"]);
  });

  getBundles(".html").forEach(function (bundle) {
    gulp.watch(bundle.inputFiles, ["min:html"]);
  });
});

gulp.task("default", [, "min"]);

function getBundles(extension) {
  return bundleconfig.filter(function (bundle) {
    return new RegExp(extension).test(bundle.outputFileName);
  });
}
