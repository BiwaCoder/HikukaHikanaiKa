# hikuka_hikanaika_core.py
# HikukaHikanaikaゲームのコア機能を提供するモジュール
# OpenAI GPT-4を使用して、リアリティーショーのキャラクター対話を実現

import os
import json
import openai

class HikukaHikanaikaCore:
    """HikukaHikanaikaゲームのメインクラス
    
    3人の女性キャラクターとのリアリティーショー対話ゲームを管理します。
    OpenAI GPT-4を使用してキャラクターの応答を生成します。
    """
    
    def __init__(self):
        """コンストラクタ - OpenAIクライアントを初期化し、キャラクター設定を読み込み"""
        # 環境変数からOpenAI APIキーを取得してクライアントを初期化
        self.client = openai.OpenAI(api_key=os.environ.get("OPENAI_API_KEY"))
        
        # キャラクター設定を読み込み
        self.character_data = self._load_character_data()
        
        # リアリティーショーのシステムプロンプト
        self.REALITY_SHOW_PROMPT = """
        あなたは『運命の館 〜HikukaHikanaika〜』の語り部です。
        夕暮れ時の古いヨーロピアンスタイルの館を舞台に、美しき女性たちが織りなす運命の物語を紡ぎます。
        
        語り部の特徴:
        - 文学的で詩的な表現を用いる
        - 心理描写と情景描写を重視
        - ドラマチックな演出で視聴者を物語に引き込む
        - 登場人物の内面の美しさや秘めた想いを描写
        - 恋愛リアリティーショーらしい心の機微を表現
        
        物語の舞台設定:
        - 夕暮れ時の「運命の館」
        - クリスタルのシャンデリアが輝く円形サロン
        - アンティーク調の家具に囲まれた優雅な空間
        - オレンジ色の陽光が差し込む神秘的な雰囲気
        
        語りのスタイル:
        1. 情景描写から始まる物語的な導入
        2. 各キャラクターの心理と美しさの深い分析
        3. 競争ではなく「美しき戦い」として描写
        4. 運命や宿命といった壮大なテーマ
        5. 余韻を残す詩的な結末
        
        美しく、ドラマチックで、視聴者の心に残る物語を創造してください。
        """

    def get_ai_response(self, messages):
        """OpenAI GPT-4に会話履歴を送信し、応答を取得
        
        Args:
            messages (list): 会話履歴のリスト
            
        Returns:
            str: AIの応答テキスト
        """
        response = self.client.chat.completions.create(
            model="gpt-4o",
            messages=messages
        )
        return response.choices[0].message.content

    def introduce_character(self, character_name, beauty_stats):
        """キャラクターの自己紹介を生成
        
        Args:
            character_name (str): キャラクター名
            beauty_stats (dict): 美容ステータス情報
            
        Returns:
            dict: キャラクターの自己紹介メッセージ
        """
        # キャラクター情報を取得
        character_info = self.character_data.get("characters", {}).get(character_name, {})
        scenario_templates = self.character_data.get("scenario_templates", {})
        narrative_elements = self.character_data.get("response_templates", {}).get("narrative_elements", {})
        
        # 美容カテゴリの詳細説明を取得
        fashion_item = beauty_stats.get('fashion', '不明')
        beauty_category_info = ""
        for category in self.character_data.get("beauty_categories", []):
            if category.get("name") == fashion_item:
                beauty_category_info = category.get("dramatic_description", "")
                break
        
        # キャラクター専用のドラマチックなシステムプロンプトを構築
        character_prompt = f"""
        あなたは『運命の館 〜HikukaHikanaika〜』の参加者、{character_info.get('name', character_name)}です。
        
        あなたの物語:
        - 年齢: {character_info.get('age', '不明')}歳
        - 職業: {character_info.get('occupation', '不明')}
        - 外見: {character_info.get('appearance', '美しい女性')}
        - 性格: {character_info.get('personality', '魅力的な人物')}
        - 特徴・趣味: {character_info.get('traits', '多才な魅力を持つ')}
        - 話し方: {character_info.get('speech_style', '上品で魅力的')}
        - 背景: {character_info.get('background', '興味深い人生を歩んできた')}
        - 美への哲学: {character_info.get('beauty_philosophy', '自分らしい美しさを追求している')}
        - 秘めた想い: {character_info.get('secret', '心の奥底に特別な想いを抱いている')}
        - キャッチフレーズ: {character_info.get('catchphrase', '')}
        
        現在の装い:
        - ファッション: {beauty_stats.get('fashion', '優雅な装い')}
        {f"- 装いの印象: {beauty_category_info}" if beauty_category_info else ""}
        
        舞台設定:
        夕暮れ時の「運命の館」。クリスタルのシャンデリアが柔らかな光を放つ円形サロンで、
        あなたは他の美しき参加者たちと共に自己紹介の時を迎えています。
        
        表現スタイル:
        - 内面の美しさと心の奥底にある想いを込めて
        - あなたの口調と性格を活かした自然な話し方で
        - ドラマチックで詩的な雰囲気を大切に
        - 300-500文字程度で心に響く自己紹介を
        - 他の参加者や視聴者の心を動かすような表現で
        """
        
        # シナリオテンプレートを取得
        intro_scenario = scenario_templates.get("introduction_scenarios", {}).get(character_name, "")
        
        messages = [
            {"role": "system", "content": character_prompt},
            {"role": "user", "content": f"""
            {intro_scenario.format(name=character_info.get('name', character_name)) if intro_scenario else ""}
            
            夕暮れの館のサロンで、クリスタルのシャンデリアの光の下、
            あなたらしい魅力的な自己紹介をしてください。
            
            内面の美しさや秘めた想い、そして現在の装いへの想いも込めて、
            他の参加者や視聴者の心に残る自己紹介をお願いします。
            """}
        ]
        
        introduction = self.get_ai_response(messages)
        
        return {
            "character_name": character_name,
            "character_info": character_info,
            "beauty_stats": beauty_stats,
            "introduction": introduction
        }

    def explain_reality_show(self, contestants):
        """リアリティーショーの解説を生成
        
        Args:
            contestants (list): 参加者リスト（各参加者の情報を含む辞書のリスト）
            
        Returns:
            dict: リアリティーショーの解説メッセージ
        """
        # 参加者情報を整理し、ドラマチックな描写を追加
        contestant_stories = []
        for contestant in contestants:
            character_name = contestant.get('character_name', 'Unknown')
            character_info = contestant.get('character_info', {})
            beauty_stats = contestant.get('beauty_stats', {})
            introduction = contestant.get('introduction', '')
            
            # 美容アイテムの詳細説明を取得
            fashion_item = beauty_stats.get('fashion', '')
            dramatic_fashion_desc = ""
            for category in self.character_data.get("beauty_categories", []):
                if category.get("name") == fashion_item:
                    dramatic_fashion_desc = category.get("dramatic_description", "")
                    break
            
            story_summary = f"""
            {character_info.get('name', character_name)} ({character_info.get('age', '不明')}歳・{character_info.get('occupation', '不明')})
            - 装い: {fashion_item}
            {f"  {dramatic_fashion_desc}" if dramatic_fashion_desc else ""}
            - 性格: {character_info.get('personality', '魅力的')}
            - 秘めた想い: {character_info.get('secret', '心に秘めた想いがある')}
            - 自己紹介: {introduction[:100]}...
            - 美への哲学: {character_info.get('beauty_philosophy', '独自の美学を持つ')}
            """
            contestant_stories.append(story_summary)
        
        # ナラティブ要素を取得
        narrative_elements = self.character_data.get("response_templates", {}).get("narrative_elements", {})
        scenario_templates = self.character_data.get("scenario_templates", {}).get("dramatic_narration", {})
        
        contest_narrative = "\n━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━\n".join(contestant_stories)
        
        messages = [
            {"role": "system", "content": self.REALITY_SHOW_PROMPT},
            {"role": "user", "content": f"""
            {scenario_templates.get('opening', '')}
            
            今回の『運命の館 〜HikukaHikanaika〜』に集った美しき参加者たちの物語を解説してください。
            
            参加者たちの物語:
            {contest_narrative}
            
            語り部として、以下の要素を含む美しく壮大な物語を紡いでください:
            
            1. 夕暮れの館の神秘的な雰囲気の描写
            2. 各参加者の内面的な美しさと秘められた想いの分析
            3. 彼女たちが纏う装いの意味と美しさの表現
            4. それぞれの美への哲学や人生観の対比
            5. 運命の糸で結ばれた彼女たちの関係性
            6. この物語の今後への期待感や余韻
            
            文学的で詩的な表現を用い、視聴者の心に深く響く物語として語ってください。
            単なる競争ではなく、美しき魂たちが織りなす運命の物語として描いてください。
            """}
        ]
        
        explanation = self.get_ai_response(messages)
        
        return {
            "contestants": contestants,
            "explanation": explanation,
            "show_title": "運命の館 〜HikukaHikanaika〜"
        }

    def get_character_list(self):
        """利用可能なキャラクターのリストを取得
        
        Returns:
            list: キャラクター名のリスト
        """
        return list(self.character_data.get("characters", {}).keys())

    def get_character_info(self, character_name):
        """指定されたキャラクターの詳細情報を取得
        
        Args:
            character_name (str): キャラクター名
            
        Returns:
            dict: キャラクター情報
        """
        return self.character_data.get("characters", {}).get(character_name, {})

    def _load_character_data(self):
        """キャラクター設定をJSONファイルから読み込み
        
        Returns:
            dict: キャラクターデータ
        """
        try:
            # 現在のファイルのディレクトリを取得
            current_dir = os.path.dirname(os.path.abspath(__file__))
            json_path = os.path.join(current_dir, "character_data.json")
            
            with open(json_path, 'r', encoding='utf-8') as f:
                return json.load(f)
        except (FileNotFoundError, json.JSONDecodeError) as e:
            print(f"警告: キャラクターデータファイルを読み込めませんでした: {e}")
            return self._get_default_character_data()

    def _get_default_character_data(self):
        """デフォルトのキャラクターデータを返す（JSONファイルが読み込めない場合のフォールバック）
        
        Returns:
            dict: デフォルトのキャラクターデータ
        """
        return {
            "characters": {
                "詩織": {
                    "name": "佐々木詩織（ささき しおり）",
                    "age": 24,
                    "occupation": "モデル",
                    "personality": "上品で知的、内面的な美しさを大切にする",
                    "appearance": "腰まで届く艶やかな黒髪、優雅な装い",
                    "traits": "読書と美術館巡りが趣味、現代アートに造詣が深い",
                    "speech_style": "上品で丁寧な口調",
                    "beauty_philosophy": "真の美しさは内面から溢れ出るもの",
                    "secret": "実は人見知りで、大勢の前では緊張してしまう",
                    "catchphrase": "美しさは、心の奥底から生まれる光のようなものです"
                },
                "るな": {
                    "name": "星野るな（ほしの るな）",
                    "age": 22,
                    "occupation": "レンタル彼女",
                    "personality": "明るく天真爛漫、場の空気を和ませる天性の才能",
                    "appearance": "肩口までの黒髪、ピンクのワンピース",
                    "traits": "カフェ巡りとショッピングが趣味、人と話すのが大好き",
                    "speech_style": "元気で親しみやすい口調",
                    "beauty_philosophy": "笑顔が一番の美容法！",
                    "secret": "実は寂しがり屋で、本当の恋愛に憧れている",
                    "catchphrase": "みんなと一緒にいると、心がぽかぽかするの！"
                },
                "愛美": {
                    "name": "高橋愛美（たかはし あいみ）",
                    "age": 26,
                    "occupation": "美容コンサルタント",
                    "personality": "クールで知的、戦略的思考を持つ完璧主義者",
                    "appearance": "ネイビーのジャケット、洗練された大人の女性",
                    "traits": "美容業界でのキャリアが長く、科学的アプローチを重視",
                    "speech_style": "冷静で論理的",
                    "beauty_philosophy": "美しさは科学と戦略の結果",
                    "secret": "完璧な外見の裏で、実は深い孤独感を抱えている",
                    "catchphrase": "美しさには必ず理論と根拠がある"
                }
            },
            "beauty_categories": [
                {
                    "name": "オートクチュールのドレス",
                    "level": 5,
                    "description": "まるで女神が纏うような、この世で最も美しいドレス",
                    "dramatic_description": "クリスタルのシャンデリアの光を受けて、ドレスが神秘的に輝いている"
                },
                {
                    "name": "制服しか勝たん",
                    "level": 4,
                    "description": "清楚で上品な制服姿。純真さの中に秘められた魅力",
                    "dramatic_description": "白いブラウスが夕暮れの光に透けて、彼女の内なる美しさを際立たせる"
                },
                {
                    "name": "量産型ガーリー",
                    "level": 3,
                    "description": "トレンドを押さえた愛らしさ。親しみやすい魅力",
                    "dramatic_description": "ピンクのワンピースが彼女の笑顔と共に、サロンに春の風を運んでくる"
                },
                {
                    "name": "清潔感あるけど量販感",
                    "level": 2,
                    "description": "清潔感は申し分ないが、どこか既視感のあるスタイル",
                    "dramatic_description": "悪くはないが、この館では埋もれてしまいそうな、控えめな装い"
                },
                {
                    "name": "サイズ合ってない",
                    "level": 1,
                    "description": "残念ながらサイズが合っていない装い",
                    "dramatic_description": "アンバランスな装いが、せっかくの美しさを台無しにしてしまっている"
                }
            ],
            "reality_show_settings": {
                "show_name": "運命の館 〜HikukaHikanaika〜",
                "concept": "古いヨーロピアンスタイルの館を舞台に、美しき女性たちが織りなす運命の物語"
            }
        }