/// <binding AfterBuild='default' Clean='clean' ProjectOpened='watch' />
"use strict";

var gulp = require("gulp"),
    concat = require("gulp-concat"),
    cleancss = require("gulp-clean-css"),
    uglify = require("gulp-uglify"),
    merge = require("merge-stream"),
    del = require("del"),
    bundleconfig = require("./bundleconfig.json"); // make sure bundleconfig.json doesn't contain any comments

var libFiles = {
  css: [
    "./node_modules/bootstrap/dist/css/bootstrap{.min,}.css"
  ],
  js: [
    "./node_modules/bootstrap/dist/js/bootstrap*",
    "./node_modules/jquery/dist/jquery{.min,}.js",
    "./node_modules/jquery-validation/dist/*.js",
    "./node_modules/jquery-validation-unobtrusive/dist/*.js"
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

gulp.task("min:js", gulp.series("copy:js", function () {
  var tasks = getBundles(".js").map(function (bundle) {
    return gulp.src(bundle.inputFiles, { base: "." })
      .pipe(concat(bundle.outputFileName))
      .pipe(uglify())
      .pipe(gulp.dest("."));
  });
  return merge(tasks);
}));

gulp.task("min:css", gulp.series("copy:css", function () {
  var tasks = getBundles(".css").map(function (bundle) {
    return gulp.src(bundle.inputFiles, { base: "." })
      .pipe(concat(bundle.outputFileName))
      .pipe(cleancss({level: 2}))
      .pipe(gulp.dest("."));
  });
  return merge(tasks);
}));

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
});



gulp.task("min", gulp.parallel("min:js", "min:css"));
gulp.task("default", gulp.series("clean", "min"));

function getBundles(extension) {
  return bundleconfig.filter(function (bundle) {
    return new RegExp(extension).test(bundle.outputFileName);
  });
}
