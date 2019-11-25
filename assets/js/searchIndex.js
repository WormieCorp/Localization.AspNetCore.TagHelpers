
var camelCaseTokenizer = function (builder) {

  var pipelineFunction = function (token) {
    var previous = '';
    // split camelCaseString to on each word and combined words
    // e.g. camelCaseTokenizer -> ['camel', 'case', 'camelcase', 'tokenizer', 'camelcasetokenizer']
    var tokenStrings = token.toString().trim().split(/[\s\-]+|(?=[A-Z])/).reduce(function(acc, cur) {
      var current = cur.toLowerCase();
      if (acc.length === 0) {
        previous = current;
        return acc.concat(current);
      }
      previous = previous.concat(current);
      return acc.concat([current, previous]);
    }, []);

    // return token for each string
    // will copy any metadata on input token
    return tokenStrings.map(function(tokenString) {
      return token.clone(function(str) {
        return tokenString;
      })
    });
  }

  lunr.Pipeline.registerFunction(pipelineFunction, 'camelCaseTokenizer')

  builder.pipeline.before(lunr.stemmer, pipelineFunction)
}
var searchModule = function() {
    var documents = [];
    var idMap = [];
    function a(a,b) { 
        documents.push(a);
        idMap.push(b); 
    }

    a(
        {
            id:0,
            title:"NewLineHandling",
            content:"NewLineHandling",
            description:'',
            tags:''
        },
        {
            url:'/Localization.AspNetCore.TagHelpers/api/Localization.AspNetCore.TagHelpers/NewLineHandling',
            title:"NewLineHandling",
            description:""
        }
    );
    a(
        {
            id:1,
            title:"Startup",
            content:"Startup",
            description:'',
            tags:''
        },
        {
            url:'/Localization.AspNetCore.TagHelpers/api/NetCoreApp21/Startup',
            title:"Startup",
            description:""
        }
    );
    a(
        {
            id:2,
            title:"HomeController",
            content:"HomeController",
            description:'',
            tags:''
        },
        {
            url:'/Localization.AspNetCore.TagHelpers/api/NetCoreApp21.Controllers/HomeController',
            title:"HomeController",
            description:""
        }
    );
    a(
        {
            id:3,
            title:"ParameterTagHelper",
            content:"ParameterTagHelper",
            description:'',
            tags:''
        },
        {
            url:'/Localization.AspNetCore.TagHelpers/api/Localization.AspNetCore.TagHelpers/ParameterTagHelper',
            title:"ParameterTagHelper",
            description:""
        }
    );
    a(
        {
            id:4,
            title:"LocalizeTagHelperOptions",
            content:"LocalizeTagHelperOptions",
            description:'',
            tags:''
        },
        {
            url:'/Localization.AspNetCore.TagHelpers/api/Localization.AspNetCore.TagHelpers/LocalizeTagHelperOptions',
            title:"LocalizeTagHelperOptions",
            description:""
        }
    );
    a(
        {
            id:5,
            title:"GenericLocalizeTagHelper",
            content:"GenericLocalizeTagHelper",
            description:'',
            tags:''
        },
        {
            url:'/Localization.AspNetCore.TagHelpers/api/Localization.AspNetCore.TagHelpers/GenericLocalizeTagHelper',
            title:"GenericLocalizeTagHelper",
            description:""
        }
    );
    a(
        {
            id:6,
            title:"LocalizeAttributeTagHelper",
            content:"LocalizeAttributeTagHelper",
            description:'',
            tags:''
        },
        {
            url:'/Localization.AspNetCore.TagHelpers/api/Localization.AspNetCore.TagHelpers/LocalizeAttributeTagHelper',
            title:"LocalizeAttributeTagHelper",
            description:""
        }
    );
    a(
        {
            id:7,
            title:"LocalizeTagHelper",
            content:"LocalizeTagHelper",
            description:'',
            tags:''
        },
        {
            url:'/Localization.AspNetCore.TagHelpers/api/Localization.AspNetCore.TagHelpers/LocalizeTagHelper',
            title:"LocalizeTagHelper",
            description:""
        }
    );
    a(
        {
            id:8,
            title:"Program",
            content:"Program",
            description:'',
            tags:''
        },
        {
            url:'/Localization.AspNetCore.TagHelpers/api/NetCoreApp21/Program',
            title:"Program",
            description:""
        }
    );
    a(
        {
            id:9,
            title:"ErrorViewModel",
            content:"ErrorViewModel",
            description:'',
            tags:''
        },
        {
            url:'/Localization.AspNetCore.TagHelpers/api/NetCoreApp21.Models/ErrorViewModel',
            title:"ErrorViewModel",
            description:""
        }
    );
    var idx = lunr(function() {
        this.field('title');
        this.field('content');
        this.field('description');
        this.field('tags');
        this.ref('id');
        this.use(camelCaseTokenizer);

        this.pipeline.remove(lunr.stopWordFilter);
        this.pipeline.remove(lunr.stemmer);
        documents.forEach(function (doc) { this.add(doc) }, this)
    });

    return {
        search: function(q) {
            return idx.search(q).map(function(i) {
                return idMap[i.ref];
            });
        }
    };
}();
