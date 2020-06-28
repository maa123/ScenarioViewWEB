'use strict';
const userConfig = {
    'username': null
};
window.writeResult = (text) => {
    document.getElementById('result').value = text;
    //document.getElementById('result').rows = (text.match(/\n/g) || []).length + 2;
    let talk = document.getElementById('talk');
    const emptytalk = talk.cloneNode(false);
    talk.parentNode.replaceChild(emptytalk, talk);
    talk = document.getElementById('talk');
    const fra = document.createDocumentFragment();
    const data = JSON.parse(text);
    for (const i of data['Base'][11]['rowDatas']) {
        //console.log(i);
        if (i['data'][0]['mType'] == 2) {
            for (let j = 0; j < 3 && i['data'][13]["mStrParams"][j]['data'] != "none" && i['data'][13]["mStrParams"][j]['data'] != ""; j++) {
                //size=30を標準とみなす
                if (i['data'][13]["mStrParams"][j]['data'].match(/※※※※※※※※※※/) && userConfig.username === null) {
                    userConfig.username = window.prompt('名前を入力してください。', 'ユーザー名');
                }
                const serifText = `${i['data'][13]["mStrParams"][j]['data'].replace(/\[(.*?):(.*?)\]/gm, "<ruby>$1<rt>$2</rt></ruby>").
                    replace(/<size=(\d+)>(.*?)<\/size>/gm, '<span style="font-size: calc($1em/30);">$2</span>').
                    replace(/<color=(#\d+)>(.*?)<\/color>/gm, '<span style="color:$1;">$2</span>').replace(/※※※※※※※※※※/, userConfig.username)}`;
                const cs = document.createElement('div');
                cs.className = 'uk-card uk-card-default uk-card-body';
                const _li = document.createElement('p');
                let _li2 = null;
                _li.innerHTML = `${i['data'][2]['mSerifCharaName']}: ${serifText}`;
                if (j == 0 && i['data'][13]["mStrParams"][2]['data'] != "" && i['data'][13]["mStrParams"][2]['data'] != "none") {
                    if (i['data'][13]["mStrParams"][1]['data'] == "none" || i['data'][13]["mStrParams"][1]['data'] == "") {
                        const serifText2 = `${i['data'][13]["mStrParams"][2]['data'].replace(/\[(.*?):(.*?)\]/gm, "<ruby>$1<rt>$2</rt></ruby>").
                            replace(/<size=(\d+)>(.*?)<\/size>/gm, '<span style="font-size: calc($1em/30);">$2</span>')
                            .replace(/<color=(#\d+)>(.*?)<\/color>/gm, '<span style="color:$1;">$2</span>').replace(/※※※※※※※※※※/, userConfig.username)}`;
                        //_li.innerHTML = `${i['data'][2]['mSerifCharaName']}: ${serifText}`;
                        //_li.classList.add('nb');
                        _li2 = document.createElement('p');
                        _li2.innerHTML = `${serifText2}`;
                        _li2.style = "font-size:0.8em;";
                        //_li2.innerHTML = `<span style="font-size:0.8em;">${serifText2}</span>`;
                    }
                }
                
                //_li.appendChild(document.createTextNode(serifText));
                cs.appendChild(_li);
                if (_li2 != null) {
                    cs.appendChild(_li2);
                }
                fra.appendChild(cs);
                
            }
        }
    }
    talk.appendChild(fra);
}