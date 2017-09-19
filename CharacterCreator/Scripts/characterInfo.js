var character = {
    id : -1,
    name: "",
    desc: "",
    player: "",
    type: "PC",
    campaign: {
        id: -1,
        name: "",
        desc: "",
        rpg: {
            id: 1,
            name: "",
            desc: "",
            template: "",
            version: 1.0,
            system: {
                id: 1,
                name: "",
                desc: "",
                dieBase: "d20"
            }
        }
    },
    common: {
        level: 1,
        sex: "",
        race: 1,
        classification: 1,
        alignment: 1
    },
    specific: { }
}

function UpdateCharacter()
{
    $('#<%= Character %>').val(character.id);
}

function AlertInfo()
{
    alert(character.name + " " + character.player + " " + character.campaign.rpg.name);
}