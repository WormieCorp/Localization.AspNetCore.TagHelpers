
var camelCaseTokenizer = function (obj) {
    var previous = '';
    return obj.toString().trim().split(/[\s\-]+|(?=[A-Z])/).reduce(function(acc, cur) {
        var current = cur.toLowerCase();
        if(acc.length === 0) {
            previous = current;
            return acc.concat(current);
        }
        previous = previous.concat(current);
        return acc.concat([current, previous]);
    }, []);
}
lunr.tokenizer.registerFunction(camelCaseTokenizer, 'camelCaseTokenizer')
var searchModule = function() {
    var idMap = [];
    function y(e) { 
        idMap.push(e); 
    }
    var idx = lunr(function() {
        this.field('title', { boost: 10 });
        this.field('content');
        this.field('description', { boost: 5 });
        this.field('tags', { boost: 50 });
        this.ref('id');
        this.tokenizer(camelCaseTokenizer);

        this.pipeline.remove(lunr.stopWordFilter);
        this.pipeline.remove(lunr.stemmer);
    });
    function a(e) { 
        idx.add(e); 
    }

    a({
        id:0,
        title:"GenericLocalizeTagHelper",
        content:"GenericLocalizeTagHelper",
        description:'',
        tags:''
    });

    a({
        id:1,
        title:"Startup",
        content:"Startup",
        description:'',
        tags:''
    });

    a({
        id:2,
        title:"LocalizeAttributeTagHelper",
        content:"LocalizeAttributeTagHelper",
        description:'',
        tags:''
    });

    a({
        id:3,
        title:"LocalizeTagHelperOptions",
        content:"LocalizeTagHelperOptions",
        description:'',
        tags:''
    });

    a({
        id:4,
        title:"NewLineHandling",
        content:"NewLineHandling",
        description:'',
        tags:''
    });

    a({
        id:5,
        title:"ParameterTagHelper",
        content:"ParameterTagHelper",
        description:'',
        tags:''
    });

    a({
        id:6,
        title:"Program",
        content:"Program",
        description:'',
        tags:''
    });

    a({
        id:7,
        title:"LocalizeTagHelper",
        content:"LocalizeTagHelper",
        description:'',
        tags:''
    });

    a({
        id:8,
        title:"HomeController",
        content:"HomeController",
        description:'',
        tags:''
    });

    y({
        url:'/Localization.AspNetCore.TagHelpers/Localization.AspNetCore.TagHelpers/api/Localization.AspNetCore.TagHelpers/GenericLocalizeTagHelper',
        title:"GenericLocalizeTagHelper",
        description:""
    });

    y({
        url:'/Localization.AspNetCore.TagHelpers/Localization.AspNetCore.TagHelpers/api/Localization.Demo/Startup',
        title:"Startup",
        description:""
    });

    y({
        url:'/Localization.AspNetCore.TagHelpers/Localization.AspNetCore.TagHelpers/api/Localization.AspNetCore.TagHelpers/LocalizeAttributeTagHelper',
        title:"LocalizeAttributeTagHelper",
        description:""
    });

    y({
        url:'/Localization.AspNetCore.TagHelpers/Localization.AspNetCore.TagHelpers/api/Localization.AspNetCore.TagHelpers/LocalizeTagHelperOptions',
        title:"LocalizeTagHelperOptions",
        description:""
    });

    y({
        url:'/Localization.AspNetCore.TagHelpers/Localization.AspNetCore.TagHelpers/api/Localization.AspNetCore.TagHelpers/NewLineHandling',
        title:"NewLineHandling",
        description:""
    });

    y({
        url:'/Localization.AspNetCore.TagHelpers/Localization.AspNetCore.TagHelpers/api/Localization.AspNetCore.TagHelpers/ParameterTagHelper',
        title:"ParameterTagHelper",
        description:""
    });

    y({
        url:'/Localization.AspNetCore.TagHelpers/Localization.AspNetCore.TagHelpers/api/Localization.Demo/Program',
        title:"Program",
        description:""
    });

    y({
        url:'/Localization.AspNetCore.TagHelpers/Localization.AspNetCore.TagHelpers/api/Localization.AspNetCore.TagHelpers/LocalizeTagHelper',
        title:"LocalizeTagHelper",
        description:""
    });

    y({
        url:'/Localization.AspNetCore.TagHelpers/Localization.AspNetCore.TagHelpers/api/Localization.Demo.Controllers/HomeController',
        title:"HomeController",
        description:""
    });

    return {
        search: function(q) {
            return idx.search(q).map(function(i) {
                return idMap[i.ref];
            });
        }
    };
}();
