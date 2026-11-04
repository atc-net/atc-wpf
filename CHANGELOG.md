# Changelog

All notable changes to this project will be documented in this file.

From 4.0.216 onwards this file is maintained by [release-please](https://github.com/googleapis/release-please) from [Conventional Commits](https://www.conventionalcommits.org/), and the project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [4.4.0](https://github.com/atc-net/atc-wpf/compare/v4.3.0...v4.4.0) (2026-11-04)


### New features

* **components:** add DialogService.ShowFontPicker ([a727e3c](https://github.com/atc-net/atc-wpf/commit/a727e3ced58ea82356c51449d535ce20344ce712))
* **components:** wrap long dialog text and size dialogs to their content ([8310357](https://github.com/atc-net/atc-wpf/commit/831035706751150c0e9c0c1bcc96ff625af40a05))
* **dialogs:** add ExceptionDialog and ExceptionReport for unhandled exceptions ([5b34e69](https://github.com/atc-net/atc-wpf/commit/5b34e694b2f6cca546b059e5748722bfc96c01ae))
* **dialogs:** share one button bar and add Yes/No/Cancel, default button, details, don't ask again and copy ([08b4424](https://github.com/atc-net/atc-wpf/commit/08b4424c249cdf4f70b01c9c2d1c9eb1106a109a))
* **sample:** add an Exception dialog sample ([a77b1f1](https://github.com/atc-net/atc-wpf/commit/a77b1f1c299984dede4856db309006ef91559af3))


### Bug fixes

* **components:** give each dialog its own copy of the settings icon ([f8454f8](https://github.com/atc-net/atc-wpf/commit/f8454f8cde63ab08c170a23ed3a7d84d6d34946c))
* **components:** size the input form dialog height to its content ([f705f1a](https://github.com/atc-net/atc-wpf/commit/f705f1a4c2abb906037e720ee50a235946ca0fcf))
* **controls:** show SplitButton and Timeline text in the theme color ([68554f1](https://github.com/atc-net/atc-wpf/commit/68554f164947fdb1c46a63c7424d787ddbf2ca75))
* **dialogs:** keep grown dialogs inside the work area ([7cb0dcc](https://github.com/atc-net/atc-wpf/commit/7cb0dcc627a2599442a24f8f02c3cd59dbf024ca))
* **forms:** own, centre and fit the colour and font picker dialogs ([b5284c6](https://github.com/atc-net/atc-wpf/commit/b5284c6d3522bae85ff34d0e5ac486eea05e8829))
* **forms:** read dialog button brushes from the current theme ([d4622c1](https://github.com/atc-net/atc-wpf/commit/d4622c1bffe69822ef396694fd58307c8c70cc59))
* **forms:** wrap long form labels and validation texts; size the input dialog to its content ([94effa7](https://github.com/atc-net/atc-wpf/commit/94effa71170d23ab6d17530876de45bb6dabd558))

## [4.3.0](https://github.com/atc-net/atc-wpf/compare/v4.2.1...v4.3.0) (2026-11-01)


### New features

* **components:** add check-box style, one-way mode, locked items, limit, grouping and virtualization to DualListSelector ([dac0446](https://github.com/atc-net/atc-wpf/commit/dac0446bcdeb4fd4db701c95ccc54a98cb1ab37f))
* **components:** add SelectedKeys, SelectedKeysChanged and MaxSelectionCount to DualListSelector ([c490cde](https://github.com/atc-net/atc-wpf/commit/c490cded523d140de05984e254c036671ab11232))
* **controls:** add a single-select mode to CascaderComboBox ([331bc9f](https://github.com/atc-net/atc-wpf/commit/331bc9fd80bd31251826b5cc012e18e6597b5235))
* **controls:** add CardSelector card/tile picker with single, single-or-none and multiple selection ([cad1e8e](https://github.com/atc-net/atc-wpf/commit/cad1e8e5ae4067d3ba6582c8895ab474fe04dc0b))
* **controls:** add CascaderComboBox multi-select drop-down with one column per level ([ee1ea26](https://github.com/atc-net/atc-wpf/commit/ee1ea26953e45564f96fc78734d325c41e6ddbea))
* **controls:** add CheckListBox with select all, filter, groups and item styles ([b421dc0](https://github.com/atc-net/atc-wpf/commit/b421dc020568d212036b4644fbd9416972f2eb92))
* **controls:** add CheckTreeView with tri-state check engine, check strategies, filter and lazy loading ([8a723a2](https://github.com/atc-net/atc-wpf/commit/8a723a228c9ab88adea0552c5b7f5c58ca1e3a14))
* **controls:** add ChipGroup with selection modes, All chip, counts and a selection limit ([7742ade](https://github.com/atc-net/atc-wpf/commit/7742ade96160f21133b1b247935663e836f09dd3))
* **controls:** add chips and count display modes, deferred commit and selection-limit tests to MultiSelectComboBox ([3e15355](https://github.com/atc-net/atc-wpf/commit/3e153557f1d0494682a83354e86081c00cebe832))
* **controls:** add DockPanelPro LayoutChanged and make AutoSaveLayout work ([d01a330](https://github.com/atc-net/atc-wpf/commit/d01a330f746315e7825c6d5f45f8a2b432f12026))
* **controls:** add Excel-style column filters, search and column chooser to DataGrid ([4555f25](https://github.com/atc-net/atc-wpf/commit/4555f25c740b93e799f51a836d53e226f92d8091))
* **controls:** add MultiColumnComboBox multi-select drop-down with a sortable check-box grid ([c087da9](https://github.com/atc-net/atc-wpf/commit/c087da9462ec4d619f06b17c6465475aabafde41))
* **controls:** add multiple and single-or-none selection, vertical layout, separate pills, icon-only and sizes to Segmented ([b6ce805](https://github.com/atc-net/atc-wpf/commit/b6ce80554fca08e07357fd16b3fa8b4ab5b24f68))
* **controls:** add MultiSelectComboBox ([4ecf1bf](https://github.com/atc-net/atc-wpf/commit/4ecf1bf4bfca461861071578652735333408a8b8))
* **controls:** add tabbed pane groups and floating tool windows to DockPanelPro ([f6fd5f8](https://github.com/atc-net/atc-wpf/commit/f6fd5f8e4d4b8529af40ffd42fab82e4bf2b2219))
* **controls:** add TokenBox with suggestions, created values, paste splitting and async lookup ([1fedcd4](https://github.com/atc-net/atc-wpf/commit/1fedcd4bd990754f9e4305d38c495b86d5a5eddf))
* **controls:** add TreeSelectComboBox with text, chips and count summaries ([80408a2](https://github.com/atc-net/atc-wpf/commit/80408a226870f379f191bee8c0f1b75dd6515cd1))
* **controls:** dock a floating DockPanelPro pane by dragging it onto a guide ([82e263e](https://github.com/atc-net/atc-wpf/commit/82e263e14aaa3fcb862fab93c6420470a596f5e9))
* **controls:** open a Popover from the keyboard and on focus, and close it when unloaded ([4feea85](https://github.com/atc-net/atc-wpf/commit/4feea854811abdbbd8fd3da89cbd70ca6cc64960))
* **controls:** persist tab groups and floating panes in the DockPanelPro layout ([d2ed464](https://github.com/atc-net/atc-wpf/commit/d2ed464c47821022181bc4d27df8a20b3a676618))
* **controls:** resize DockPanelPro splitters with the keyboard ([c89f9a0](https://github.com/atc-net/atc-wpf/commit/c89f9a093344cd79debee26a344503b26161a618))
* **controls:** save and restore DataGrid column layout and filters ([39b68bd](https://github.com/atc-net/atc-wpf/commit/39b68bda73462d45834d7590f2af6be406600380))
* **core:** drag the whole selection and auto-scroll drop targets at their edges ([d3a200d](https://github.com/atc-net/atc-wpf/commit/d3a200db7e6911017b38a733851a51f5e399f3a1))
* **core:** report SvgImage load failures through an ImageFailed event instead of throwing ([aefdb7b](https://github.com/atc-net/atc-wpf/commit/aefdb7b25fb00e81462696cd6298003c9ef7740f))
* **forms:** add LabelCascaderComboBox with mandatory and limit validation ([a568746](https://github.com/atc-net/atc-wpf/commit/a568746030ce96648d0572ec366113f2062ddb9e))
* **forms:** add LabelCheckListBox ([eae6ccf](https://github.com/atc-net/atc-wpf/commit/eae6ccf19b35d3afe37e2277947e903b3074710b))
* **forms:** add LabelChipGroup ([523961a](https://github.com/atc-net/atc-wpf/commit/523961a58a74dbbf3179634553196aa374199a54))
* **forms:** add LabelMultiColumnComboBox with mandatory and limit validation ([c20ae3e](https://github.com/atc-net/atc-wpf/commit/c20ae3efeba05dceca7b9f7114669094ac5772d4))
* **forms:** add LabelMultiSelectComboBox ([18a4711](https://github.com/atc-net/atc-wpf/commit/18a4711a36952debf6dd483c8c917aa7dc73d0e9))
* **forms:** add LabelTokenBox ([3950b26](https://github.com/atc-net/atc-wpf/commit/3950b2695c7a4f39d2c71c769e0d4adcc4876dff))
* **forms:** add LabelTreeSelectComboBox ([52cc5d8](https://github.com/atc-net/atc-wpf/commit/52cc5d87f1169b5be7d4c9f6380d01d50422e300))
* **forms:** generate multi-select fields for [Flags] enum and collection properties ([d50a627](https://github.com/atc-net/atc-wpf/commit/d50a62714ef316c1fe0be84fe41702f879f6eda1))
* **forms:** pass display mode, chips and commit mode through LabelMultiSelectComboBox and validate the selection limit ([416d450](https://github.com/atc-net/atc-wpf/commit/416d4508e4e37f6772b45112bffb8fcbe0bd34e8))
* **network:** send VNC keys from the typed character for any keyboard layout ([4fa6c0b](https://github.com/atc-net/atc-wpf/commit/4fa6c0b87105fbcfcc2192a9f6af186b295ae066))
* **sample:** add DataGridFilter sample with 5,000 and 100,000 row grids ([426fc26](https://github.com/atc-net/atc-wpf/commit/426fc267aa998d105402a40504e23d0a777e3cc0))


### Bug fixes

* **animation:** complete the *Async helpers when superseded, and add cancellation ([4cb833c](https://github.com/atc-net/atc-wpf/commit/4cb833cb806a9e10dcbf5956962bfbf9964d5c2a))
* **behaviors:** scroll the auto-scroll list view once per frame ([ee8ae2a](https://github.com/atc-net/atc-wpf/commit/ee8ae2aeccd649731766c6d4d4d13e24c881ab4b))
* **components:** bound the TerminalViewer and ApplicationMonitor backlog and clear it safely ([02d7ba6](https://github.com/atc-net/atc-wpf/commit/02d7ba6092b112842036315e0e0cb88c62f26de1))
* **components:** close InfoDialogBox on Escape and own DialogService dialogs by the active window ([f3f2cdf](https://github.com/atc-net/atc-wpf/commit/f3f2cdf9e05803752a994af59754365923c8c320))
* **components:** drop a busy session's Cancel button when it ends while the region stays busy ([dca44ef](https://github.com/atc-net/atc-wpf/commit/dca44ef2be8c7543d6c2a4b1048459edc071ae7e))
* **components:** give DualListSelector its own collection views ([61e135b](https://github.com/atc-net/atc-wpf/commit/61e135b3996d4c5ca0de727a5277b7256f8d0771))
* **components:** keep ApplicationMonitor view model settings and scroll only its own view ([7f05919](https://github.com/atc-net/atc-wpf/commit/7f05919db6bdd05d563e739897b111ced1abbf8c))
* **components:** keep JsonViewer value text from getting a negative width ([2ece308](https://github.com/atc-net/atc-wpf/commit/2ece30825617fc7bd7bce35366901519b2460467))
* **components:** lay out a previewed print job again for the printer chosen after the preview ([4b0635c](https://github.com/atc-net/atc-wpf/commit/4b0635c7c59a4f21d389af3943bd3499003264ec))
* **components:** let BusyIndicatorService.RunAsync rethrow cancellations it did not cause ([391136e](https://github.com/atc-net/atc-wpf/commit/391136e8e3d77447371d8641896406459f88ca97))
* **components:** let DualListSelector release its collection views when unloaded ([2180dfa](https://github.com/atc-net/atc-wpf/commit/2180dfa809f11189aa821d9d0b4b34f948bdc1a8))
* **components:** make ToastNotification.CloseOnClick close the toast and pause expiry on hover ([57c2334](https://github.com/atc-net/atc-wpf/commit/57c23340e9e9781a087a3c1709711ea006537b20))
* **components:** raise Flyout Closing once on light dismiss and keep a cancelled close from reopening ([58ddd77](https://github.com/atc-net/atc-wpf/commit/58ddd775828ac32cbfab37fe8ea5e9d0823cd015))
* **components:** stop assuming a main window when printing and opening dialogs ([c73ccd6](https://github.com/atc-net/atc-wpf/commit/c73ccd6c4567e00e3a6c5004b6a55ec6a39d695e))
* **components:** stop DualListSelector rows from logging binding errors ([54e4601](https://github.com/atc-net/atc-wpf/commit/54e4601fcfc400d0dc6344a2ada4311e4dfcabeb))
* **components:** track service-added flyouts in FlyoutHost and release cancellation registrations ([f790865](https://github.com/atc-net/atc-wpf/commit/f7908657cb5a3485d4456ca3804c28eada72a69b))
* **components:** update the JsonViewer key colour on a theme change ([2077a18](https://github.com/atc-net/atc-wpf/commit/2077a182593816c4d3e005767814a006b895e1c4))
* **controls:** align Escape and the pop-up width across the multi-select drop-downs ([b8f4dfa](https://github.com/atc-net/atc-wpf/commit/b8f4dfa784c7094ad6befbd5bfb9bb19a8c5f83a))
* **controls:** clean the MultiSelectComboBox selection of an open deferred draft after the bindings settle ([2b745a2](https://github.com/atc-net/atc-wpf/commit/2b745a2eec05fb7cd179d0b0c713a72035b66f5c))
* **controls:** clip the ZoomBox drag-zoom rectangle to the content, not the overlay ([7634b97](https://github.com/atc-net/atc-wpf/commit/7634b97393bdc6e2a6f9570604be8a7c32f72865))
* **controls:** close the MultiSelectComboBox pop-up on outside clicks after ticking ([7151186](https://github.com/atc-net/atc-wpf/commit/7151186ced1494be6ad5d8f76bb0a004d7f5630b))
* **controls:** close the TokenBox suggestions on an outside click or scroll ([f69bde6](https://github.com/atc-net/atc-wpf/commit/f69bde6e168c7b226766f31366066689b264afdd))
* **controls:** combine the filters of DataGridFilter grids that share one collection view ([3c1d8df](https://github.com/atc-net/atc-wpf/commit/3c1d8dfedda58774ca605f1b1a9189c29eeeb2ba))
* **controls:** compare multi-select keys as sets so a reorder raises no SelectedKeysChanged ([3ff50fd](https://github.com/atc-net/atc-wpf/commit/3ff50fdf2cc9e40d0630dadc431aa2da57b700a1))
* **controls:** convert each monitor's work area with its own DPI ([6c5d181](https://github.com/atc-net/atc-wpf/commit/6c5d1815af9febf15c1aa5a261458c8ed75ada0f))
* **controls:** draw DockRegion header and tab button icons in the header text colour ([cd6ff80](https://github.com/atc-net/atc-wpf/commit/cd6ff80261768c20fae943ba57bc5f15102c529f))
* **controls:** drop stale keys from the MultiSelectComboBox selection when items change during a deferred draft ([3287fae](https://github.com/atc-net/atc-wpf/commit/3287fae77668870ba072a7e79fbb1bf21f2b8b8c))
* **controls:** drop TokenBox tokens of items removed from Items while AllowCreate is set ([4f629a5](https://github.com/atc-net/atc-wpf/commit/4f629a52f37348b421e20921ee0ef806f1630f62))
* **controls:** end the TokenBox busy state when the suggestions provider throws its own cancellation ([ddfdd37](https://github.com/atc-net/atc-wpf/commit/ddfdd3727a0fa62be31249a6dd1f68366bbb0ada))
* **controls:** filter DataTable, DataView and dynamic rows in DataGridFilter ([3e9c79f](https://github.com/atc-net/atc-wpf/commit/3e9c79f6194c99376e73acf420d08f0a9b9b3233))
* **controls:** handle a cleared NavigationView item list and stop the navigation service rooting the view ([4349304](https://github.com/atc-net/atc-wpf/commit/4349304f3059592d54cb24d7a18376881a59efc2))
* **controls:** hide DockPanelPro floating windows while the panel is unloaded ([24cb3e6](https://github.com/atc-net/atc-wpf/commit/24cb3e644b86905b5fdf285fca1682913f336096))
* **controls:** honour ZoomRuler.FontSize, keep its major ticks, and stop re-fitting a zoomed ZoomBox ([1ce3dca](https://github.com/atc-net/atc-wpf/commit/1ce3dcabb8d5f8d9e049a2f22c1b3c3af170decf))
* **controls:** keep a read-only NumericBox value selectable and its spin buttons off ([46ba6bf](https://github.com/atc-net/atc-wpf/commit/46ba6bf3634f1ad8ae2fa2e5a3ebc2fa91a14ab9))
* **controls:** keep ancestor resources, RTL, language and font in floating DockPanelPro panes ([09a94d7](https://github.com/atc-net/atc-wpf/commit/09a94d736b2912e1c2aa71a5a86f7471186eae47))
* **controls:** keep floating DockPanelPro panes in their windows on layout load and keep their DataContext ([66a5bf2](https://github.com/atc-net/atc-wpf/commit/66a5bf2d72dcd2715c6b0d1afa7cb853fa88666c))
* **controls:** keep one key in a single-mode CardSelector ([765e0da](https://github.com/atc-net/atc-wpf/commit/765e0daed188d0007066d55de7016d35874ca214))
* **controls:** keep only the most recent ChipGroup key when switching from Multiple to a single selection mode ([a274562](https://github.com/atc-net/atc-wpf/commit/a2745628f18a07e27b5c34bf8836b9f476e15f61))
* **controls:** keep restored multi-select keys until their items disappear ([d2a689e](https://github.com/atc-net/atc-wpf/commit/d2a689e9f89f18ef183346734262ceda978addf6))
* **controls:** keep the check tree engine consistent after a duplicate key ([cec35dd](https://github.com/atc-net/atc-wpf/commit/cec35dde75f15f2f71040240ecb46c362f8893a3))
* **controls:** keep the DataGridFilter button when a header template is set later ([72ad628](https://github.com/atc-net/atc-wpf/commit/72ad628e60378a3313920db7955ddedf639b727f))
* **controls:** keep the hidden values of a DataGridFilter values filter on OK ([8e55a5a](https://github.com/atc-net/atc-wpf/commit/8e55a5a7d24883c2d024a3a5e28d64216fa93ac9))
* **controls:** keep the inherited DataContext on the country, language and font selectors and WellKnownColorPicker ([bd12311](https://github.com/atc-net/atc-wpf/commit/bd123113dbc455c0d06646a900288355b86f67b4))
* **controls:** keep the newest CascaderComboBox column in view when columns scroll ([fb2bca1](https://github.com/atc-net/atc-wpf/commit/fb2bca167716224d44894a2b92ebc047d97a446f))
* **controls:** keep the NumericBox value while editing a currency or unit format ([de05c16](https://github.com/atc-net/atc-wpf/commit/de05c166b02f709cb40020798536258955343ad6))
* **controls:** let Clear and untick drop kept keys of unloaded tree nodes ([eafb6de](https://github.com/atc-net/atc-wpf/commit/eafb6deec5cbe187c9517d1834465103a5e1874c))
* **controls:** let Clear drop multi-select keys that have no item ([aff6c08](https://github.com/atc-net/atc-wpf/commit/aff6c085e1aa7a0f9001cd9592ce6c4d79d55746))
* **controls:** let DataGridFilter wait for a row edit or add before it filters ([b5b196f](https://github.com/atc-net/atc-wpf/commit/b5b196f8a3a285f325fc0b69431b01638aa4bc49))
* **controls:** let text inputs inside a ZoomBox receive Space and AltGr characters ([7d20454](https://github.com/atc-net/atc-wpf/commit/7d2045400c48beaf475a3e47f38d9aea12fb8399))
* **controls:** let the NumericBox mouse wheel scroll a form instead of changing values ([c76feaf](https://github.com/atc-net/atc-wpf/commit/c76feaf94175350f817addfc521cc029d071b905))
* **controls:** listen weakly to the key, node and row collections of the multi-select controls so a long-lived collection never keeps a control alive ([6e2eafe](https://github.com/atc-net/atc-wpf/commit/6e2eafeba828be31c182562be51f9672a60eb99f))
* **controls:** make the Segmented selection follow its keys when segments change and drop keys of removed segments ([4bb9183](https://github.com/atc-net/atc-wpf/commit/4bb91835a799be8c295b68bdf3bed30bda9aadbb))
* **controls:** measure ResponsivePanel children at their spanned width ([0d2166b](https://github.com/atc-net/atc-wpf/commit/0d2166b3af619919ae41b934aea32f577385ef62))
* **controls:** mirror the TokenBox token arrows in right-to-left layouts ([1b8fc90](https://github.com/atc-net/atc-wpf/commit/1b8fc90b6317ef1f310c78b8634caf4e891db06d))
* **controls:** move focus with the Segmented selection and use one tab stop ([724a6dc](https://github.com/atc-net/atc-wpf/commit/724a6dc9c1411a7c55017ee650a54d05ba93ab1f))
* **controls:** move the focus with a DockPanelPro pane that is closed, floated or docked back ([719244f](https://github.com/atc-net/atc-wpf/commit/719244f4dbe898ccda4333c7b366c50098ac3584))
* **controls:** pick the monitor in device pixels before converting its work area ([1b9370c](https://github.com/atc-net/atc-wpf/commit/1b9370c6b357cb64335e4528c01d9a24793d55a3))
* **controls:** raise DockRegion expand/collapse automation events and check enabled ([1bcfff3](https://github.com/atc-net/atc-wpf/commit/1bcfff397fc527bccc113b61c15fd670c517a190))
* **controls:** raise UI Automation state changes for check tree and cascader rows ([82dba36](https://github.com/atc-net/atc-wpf/commit/82dba36751ce8f02d9bcc35d7f23b05ff131b2c5))
* **controls:** raise UI Automation value changes from NumericBox and Rating ([4fb6f33](https://github.com/atc-net/atc-wpf/commit/4fb6f334aef93afa2db693de771fa3e61339c776))
* **controls:** refresh a hidden floating window's ancestor resources when it shows again ([8e7dc72](https://github.com/atc-net/atc-wpf/commit/8e7dc72b96804e2a43e7628f1dba20b9e0656499))
* **controls:** refresh the DataGrid view on a column chooser change only when a search is active ([9afd2fa](https://github.com/atc-net/atc-wpf/commit/9afd2fa06856a4a0df8744e1af34a3330386dd15))
* **controls:** reject a MaxSelectionCount below one across the multi-select family ([91ea024](https://github.com/atc-net/atc-wpf/commit/91ea0248ba3d96e15a98d820aff8586cefae1d1b))
* **controls:** reshape SelectedKeys when the tree key strategy changes at runtime ([281e825](https://github.com/atc-net/atc-wpf/commit/281e825a241836f12a80098521a5fc4169c4c0d8))
* **controls:** run the stale-key cleanup of bound multi-select controls after the bindings settle so a DataContext swap never writes into the previous view model ([caea41e](https://github.com/atc-net/atc-wpf/commit/caea41e750b434b23a73c07eae17711a9c0dae8f))
* **controls:** run ZoomCommandMessage before any zoom command was used ([9e9662d](https://github.com/atc-net/atc-wpf/commit/9e9662d5219aa3a4c6dfe780a994ae8bcbd2af21))
* **controls:** sort country and language names with the UI culture's rules ([0a91df7](https://github.com/atc-net/atc-wpf/commit/0a91df70133d49b55bacb52a19cb0e97bfd46dcd))
* **controls:** stop AutoGrid placing a child on top of adjacent spanning cells ([7dc8b6b](https://github.com/atc-net/atc-wpf/commit/7dc8b6b852165add0db9d85b88ce81b1c1411763))
* **controls:** stop NumericBox overwriting an explicit Culture on UI-culture change ([cac04e2](https://github.com/atc-net/atc-wpf/commit/cac04e2127f2d044c4465e17fd0d17c84d08cb79))
* **controls:** stop SelectNewItems from ticking items in a swapped-in view model ([4850d29](https://github.com/atc-net/atc-wpf/commit/4850d29345e8a37247a30862483af42d453f5f1c))
* **controls:** stop the NumericBox hex pattern from backtracking on long digit runs ([62f342b](https://github.com/atc-net/atc-wpf/commit/62f342b235422e4c7dc5425d53e7294b83780a78))
* **controls:** stop TokenBox from writing new values into a shared Items catalog ([ca4f9eb](https://github.com/atc-net/atc-wpf/commit/ca4f9eb4cb718c2d799dd6b4b45f1fb2e54ba0f2))
* **controls:** take Ctrl+A in CardSelector before the ListBox select-all command ([c0ab124](https://github.com/atc-net/atc-wpf/commit/c0ab124411c3381ef797c9b33a1c2feee708d633))
* **controls:** theme the DataGrid filter funnel from the header, keep Alt+Down for editing cells and count rows without the new-item row ([cdd2d74](https://github.com/atc-net/atc-wpf/commit/cdd2d7485b96d7213394201888ae77ba8cc2bfc0))
* **controls:** throw a clear exception when two check-tree nodes share a key ([33b5c79](https://github.com/atc-net/atc-wpf/commit/33b5c79f33dafbcd1c7029788ebd81d007f17bd0))
* **controls:** tick new items only up to the selection limit and skip SelectedKeys updates that change nothing ([7abce30](https://github.com/atc-net/atc-wpf/commit/7abce30b80ae4b6a02db1357aca03985e340e14f))
* **controls:** toggle CheckTreeView rows on a click anywhere on the row and expand the sub-tree on '*' ([637c5c1](https://github.com/atc-net/atc-wpf/commit/637c5c134d795230f90613e364a4037edc5eec45))
* **controls:** translate saved DataGridFilter values filters to the grid's culture ([f00e0e8](https://github.com/atc-net/atc-wpf/commit/f00e0e8d15cc05daf14ba8ceab3b889feec9586d))
* **controls:** treat a focused DockPanelPro tab or header button as focus in its pane ([c7d6db5](https://github.com/atc-net/atc-wpf/commit/c7d6db5dc2fbe418776c64207c51f9f7e8200678))
* **controls:** trim a bound Segmented selection to one key only after the bindings settle when leaving the multiple mode ([6cdb195](https://github.com/atc-net/atc-wpf/commit/6cdb1954ae05f9528deb4b28c5676c6fe4ed9e1c))
* **controls:** unhook Segmented items on Items.Clear() ([78bb981](https://github.com/atc-net/atc-wpf/commit/78bb9816755f7d975004f4aa8e3d372bdaec92cd))
* **core:** apply PanelHelper spacing on the main axis only and skip collapsed children ([ec00d0c](https://github.com/atc-net/atc-wpf/commit/ec00d0cd9f1d66bc61514a131b68e8785e58aeb7))
* **core:** bound SVG parsing and rendering so hostile files cannot hang or crash the app ([f133e00](https://github.com/atc-net/atc-wpf/commit/f133e0037fd1d542e44af467e168b1a2b6692c3e))
* **core:** compute the drop index from the item under the pointer, along the panel's flow, in source-collection terms ([002383f](https://github.com/atc-net/atc-wpf/commit/002383f9a5cf3ed5187509f19b5e5580e4599574))
* **core:** drag the item under the press point and never start a drag from text boxes or scroll bars ([bb704d6](https://github.com/atc-net/atc-wpf/commit/bb704d68f65c216e2f4b5bf800750806e680bf04))
* **core:** keep fractional SVG dashes and load SVG images without locking their files ([c7c85c2](https://github.com/atc-net/atc-wpf/commit/c7c85c2e79f55861a8242e12a8478306308365c2))
* **core:** keep JSON string escapes in JsonFormatter so edited text stays valid JSON ([aaadf12](https://github.com/atc-net/atc-wpf/commit/aaadf124a69e7a0e34645f3676c96e73998c660d))
* **core:** keep SVG paint inheritance linear and render shared brushes frozen ([2cb4eb8](https://github.com/atc-net/atc-wpf/commit/2cb4eb8c1f07fc7b92617c5844aab2bf06265ef6))
* **core:** let buttons inside a multi-selected item receive the press ([d712af1](https://github.com/atc-net/atc-wpf/commit/d712af12cc0683cb805925889add8c0c311fd05c))
* **core:** let HotkeyService accept letter and digit hotkeys without a modifier and marshal registrations to its dispatcher ([13ca414](https://github.com/atc-net/atc-wpf/commit/13ca414fb432df2d3fa94395add1f3f1a2ed0233))
* **core:** let ObservableCollectionEx pass single changes to bound collection views instead of refreshing them ([29c05fe](https://github.com/atc-net/atc-wpf/commit/29c05fe769d6dc40b76bea46005f21ae40b3f534))
* **core:** make CultureManager.SetCultures set the UI culture it is given and keep the sync flag ([cd8011b](https://github.com/atc-net/atc-wpf/commit/cd8011b6096ac8e9aec12a5d5a218dd47e927fa1))
* **core:** make DebounceDispatcher.Throttle fire at every interval while calls keep coming ([825d1c2](https://github.com/atc-net/atc-wpf/commit/825d1c2516b4c56085b58494d92ca5c0c3af6dea))
* **core:** split GridEx spacing over the cells on both sides of each gap ([723fb32](https://github.com/atc-net/atc-wpf/commit/723fb32f852f5c38dde88f0cb1e2f96315647586))
* **core:** StringValidationRule.Required rejects an empty value ([90f9e57](https://github.com/atc-net/atc-wpf/commit/90f9e57f4739333b0ec9ac1e19ddedd866091b3c))
* **fonticons:** apply Rotation to static ImageMaterialDesign, ImageWeather, ImageIcoFont and ImageBootstrap icons ([244452d](https://github.com/atc-net/atc-wpf/commit/244452df09a08c03eff8ffb56e8e4a73448cbce2))
* **forms:** extract NaN, infinite and huge doubles and unbounded [Range] without throwing ([e6c7ccf](https://github.com/atc-net/atc-wpf/commit/e6c7ccff0dc9befed59d7e09950c85c4a7df3a10))
* **forms:** keep integer box values as int in the form key values ([d977e6b](https://github.com/atc-net/atc-wpf/commit/d977e6bfb9b6186da607701dbc2899f169070cb4))
* **forms:** keep long, ulong and uint values in the PropertyGrid integer editor ([4ef4229](https://github.com/atc-net/atc-wpf/commit/4ef422900c22e05ffb4ee4a9a5bdb73756f7a71d))
* **forms:** keep wide integers, bound integer fields by their type and allow unselected enums ([3f19e5c](https://github.com/atc-net/atc-wpf/commit/3f19e5cbcc6cc7d6d5be86df967f2086d181b7ef))
* **forms:** PropertyGrid category border follows theme changes and no longer requires the theme ([eca70b3](https://github.com/atc-net/atc-wpf/commit/eca70b3d8283b2a0c67869c698fe76ef362e2783))
* **forms:** PropertyGrid skips indexers instead of throwing TargetParameterCountException ([53eb72e](https://github.com/atc-net/atc-wpf/commit/53eb72e2eb9200ab1988be4a6b91d84f53e75522))
* **forms:** validate the Label multi-select wrappers the same way, including the limit and in-place key changes ([a3d52ca](https://github.com/atc-net/atc-wpf/commit/a3d52ca2e6c0e10a3df5ed15f4e33d8a0b8b37a5))
* **forms:** write empty dates as null and keep DateOnly, TimeOnly and DateTimeOffset types ([bd0b0e6](https://github.com/atc-net/atc-wpf/commit/bd0b0e6f026cf305e336f375d51a5ac83eba2092))
* **hardware:** drop exited processes and closed windows from the pickers ([ba8723d](https://github.com/atc-net/atc-wpf/commit/ba8723da137a179eae0be5354d3dbfe021e6c3be))
* **hardware:** follow the Windows default audio device and camera updates ([c92caef](https://github.com/atc-net/atc-wpf/commit/c92caef98bc654ba8adc74a19f48a7ef346f85f5))
* **hardware:** keep a shared service watching until its last picker unloads ([5c8e1f0](https://github.com/atc-net/atc-wpf/commit/5c8e1f07709149765789aeb360650b640973bb9d))
* **hardware:** let an undisposed watcher service be collected ([932da14](https://github.com/atc-net/atc-wpf/commit/932da143ee49c0251c2da9ddc5e851f0af8185d6))
* **hardware:** never lose a device watcher restart while it is stopping ([cf7f0f5](https://github.com/atc-net/atc-wpf/commit/cf7f0f5b312ae0b9f1f7e836ab126699f2856315))
* **hardware:** take the default audio marker from Windows on refresh and add ([ca3b818](https://github.com/atc-net/atc-wpf/commit/ca3b8185598bef69781662e07fd8b9c43133cd04))
* **network:** always unlock the VNC framebuffer and clip updates to the bitmap ([3f8ae89](https://github.com/atc-net/atc-wpf/commit/3f8ae89c1b5376391111dc43986cba4f78dced25))
* **network:** cancel a running scan on dispose and coalesce progress reports ([f0c0fd7](https://github.com/atc-net/atc-wpf/commit/f0c0fd78e63678987ba35cb272df597d925d2345))
* **network:** keep VNC pointer buttons in sync during drags and forward the wheel ([2fe17c4](https://github.com/atc-net/atc-wpf/commit/2fe17c454ce48c05e930a1e9d34e644bdaaa77a0))
* **theming:** carry the DataGrid row header visibility and row details scrolling to the row template by inheritance, so rows recycled during a filter refresh log no binding errors ([6a86d0a](https://github.com/atc-net/atc-wpf/commit/6a86d0a474093bdd6f167e1ad5253414f35c753b))
* **theming:** give the StatusBar style a theme background outside a NiceWindow ([bef6d50](https://github.com/atc-net/atc-wpf/commit/bef6d509fceabcaff4f046a0892c616e8ae87674))
* **theming:** restore a saved window placement only when its title bar is on a monitor ([3e6d4c5](https://github.com/atc-net/atc-wpf/commit/3e6d4c569dbe6968c2e7d3746e35af107ff473d2))
* **theming:** set WINDOWPLACEMENT length when restoring a saved window placement ([90fd86e](https://github.com/atc-net/atc-wpf/commit/90fd86ec7b6bb9066f9c19a90cbe061c482d6d82))
* **theming:** show a keyboard focus outline on themed tab headers ([24d43fc](https://github.com/atc-net/atc-wpf/commit/24d43fce5c7dc461a29a869c3ac887b5ba4cfa65))
* **undoredo:** update the history rows in place instead of rebuilding the list ([cac7941](https://github.com/atc-net/atc-wpf/commit/cac79418b1e161e767286a538dba6629f3398956))


### Performance improvements

* **components:** filter the TerminalViewer search incrementally and survive regex timeouts ([9da20fb](https://github.com/atc-net/atc-wpf/commit/9da20fbcc2e4fdcbcf0a5a7e3932079f15a95ba8))
* **components:** move items in DualListSelector with one count update and one Reset per list ([2f3fab1](https://github.com/atc-net/atc-wpf/commit/2f3fab15f0d17782bffabde22b635761e2f84cd3))
* **components:** trim TerminalViewer lines one by one while streaming at the cap ([7d17afa](https://github.com/atc-net/atc-wpf/commit/7d17afa6373ee02a39edeee2e4e212b911f0a026))
* **components:** virtualize the JsonViewer tree and keep its expand state across theme changes ([f480b32](https://github.com/atc-net/atc-wpf/commit/f480b32275db5a68a2938b311bf31d317bac5d43))
* **controls:** index appended tree nodes incrementally and add DeferRefresh ([bbea9d1](https://github.com/atc-net/atc-wpf/commit/bbea9d13a6654d3a47022e9702aaa94b226aab11))
* **controls:** update the multi-select summaries, chips and check states once per bulk selection change ([e3a7167](https://github.com/atc-net/atc-wpf/commit/e3a716758d4776e6fb883acf1819482cc9713270))
* evict the least recently used SVG drawing instead of clearing the whole cache ([5503e86](https://github.com/atc-net/atc-wpf/commit/5503e86496f3d0e8603290b8986910eb61dd7030))
* **fonticons:** render Image* icons as shared vector images ([d3c036d](https://github.com/atc-net/atc-wpf/commit/d3c036d2231cfcd1cf7310f413ffbec3e0a490a2))
* **hardware:** find process main windows in one EnumWindows pass ([3a0a1c6](https://github.com/atc-net/atc-wpf/commit/3a0a1c67d970297ce8422acb6cb9c3f9b7742d5a))
* **hardware:** let the capture pipeline scale camera preview frames ([97efb65](https://github.com/atc-net/atc-wpf/commit/97efb65f541d4b8b3feaffe760db6170bd3066e0))
* **hardware:** reuse the audio meter waveform point collections ([2bfcb0b](https://github.com/atc-net/atc-wpf/commit/2bfcb0b24a66b3f833a2490646a7c051c9ed4b10))
* **network:** render queued VNC rectangles in one dispatcher callback ([2590a4f](https://github.com/atc-net/atc-wpf/commit/2590a4fcfc71f120ca6f751b2574b386d23790f9))
* parse JSON once in JsonFormatter ([66bcec3](https://github.com/atc-net/atc-wpf/commit/66bcec3a92deb9a35fdaf1555561a267c6437032))

## [4.2.1](https://github.com/atc-net/atc-wpf/compare/v4.2.0...v4.2.1) (2026-10-17)


### Bug fixes

* **components:** close only the topmost flyout on Escape ([fe49990](https://github.com/atc-net/atc-wpf/commit/fe4999023b0b822b20107ff01ae6bc7a85176bd1))
* **core:** prune collected targets from managed markup extensions ([9dafdc9](https://github.com/atc-net/atc-wpf/commit/9dafdc9836b4d95eeb862fc0ff87e16171d329da))
* **forms:** keep invalid picker text and report full date-time on lost focus ([2b0469d](https://github.com/atc-net/atc-wpf/commit/2b0469d1c6e903755464860f2c57170d52abc5d8))
* **forms:** treat the enum picker placeholder as missing when mandatory ([9de5610](https://github.com/atc-net/atc-wpf/commit/9de5610c5b8f83659e1a38e469e93f734e556999))
* **hardware:** apply polling service results on the UI thread ([43aee4c](https://github.com/atc-net/atc-wpf/commit/43aee4c76edfee2ffc4537583254bbf0a3218cc3))
* **hardware:** stop live previews without blocking in Dispose ([c19e9b8](https://github.com/atc-net/atc-wpf/commit/c19e9b8ac88f7f00945ae1d5f8fa103993263694))
* **layouts:** re-apply GridEx and PanelHelper spacing without overwriting user margins ([0dfda3a](https://github.com/atc-net/atc-wpf/commit/0dfda3a11f95e890aa7b288ea704d4038f41d4e3))
* **theming:** bind the validation error text to the current error ([078f733](https://github.com/atc-net/atc-wpf/commit/078f733ff7aef1d318e8aad51c84417c53d5fbc0))

## [4.2.0](https://github.com/atc-net/atc-wpf/compare/v4.1.0...v4.2.0) (2026-10-16)


### New features

* **sample:** make the readme markdown readable in the dark theme ([dce6391](https://github.com/atc-net/atc-wpf/commit/dce6391d79173686995d8d259313a3238e5e09e6))


### Bug fixes

* **components:** correct a hang, a drop index, highlight, trim and dispose defects ([670850a](https://github.com/atc-net/atc-wpf/commit/670850a6a087bac5160d33ed904aec0257fccbf9))
* **components:** keep the DualListSelector footer counts in sync with the collections ([b297e63](https://github.com/atc-net/atc-wpf/commit/b297e6327112bfba47b5803f6b4d2b51a8bc572a))
* **controls:** arrange StaggeredPanel in the measured column count and clamp panel sizes ([06c0583](https://github.com/atc-net/atc-wpf/commit/06c0583015a715bf7019e50f78204b7896a3146b))
* **controls:** attach the NumericBox text handlers only once ([3a37123](https://github.com/atc-net/atc-wpf/commit/3a371233a909854c06cb0e755b7362a4c34528c0))
* **controls:** clip AutoGrid spans at the last row and column ([3cabedf](https://github.com/atc-net/atc-wpf/commit/3cabedf94872c3554d8ed5b24688fe610bad6e27))
* **controls:** correct ZoomBox content changes, fit animation and MakeVisible ([0e81540](https://github.com/atc-net/atc-wpf/commit/0e815409e0ae99aa53ce1a991e55bf3b2a34c1ea))
* **controls:** hide ToggleSwitch header and content presenters for empty strings ([f58f15a](https://github.com/atc-net/atc-wpf/commit/f58f15ab251e0b0b50001b83958a0d7213ab462a))
* **controls:** keep a custom BusyOverlay text when the UI culture changes ([f5fd40a](https://github.com/atc-net/atc-wpf/commit/f5fd40a0372d2f08a785da3ab664868e27808bd8))
* **controls:** keep ZoomGridOverlay major lines on fixed content positions ([d0d7401](https://github.com/atc-net/atc-wpf/commit/d0d7401b39f6a710584f7a635c60e7839c4d11eb))
* **controls:** make GridLines safe for non-positive steps and without a main window ([b561c69](https://github.com/atc-net/atc-wpf/commit/b561c69da2c2e828d8b0031e6991c79a344d244c))
* **controls:** make the Row panel measure robust ([eb1b43b](https://github.com/atc-net/atc-wpf/commit/eb1b43bc750226b8c91bf03aa82f7201ef183b2c))
* **controls:** realize the contiguous index range in VirtualizingStaggeredPanel ([1d7ae86](https://github.com/atc-net/atc-wpf/commit/1d7ae86e73eb26f64ac461360479f176ee558d8c))
* **controls:** restore only docking dimensions in DockPanelPro.LoadLayout ([95df764](https://github.com/atc-net/atc-wpf/commit/95df764788487a161e73587b3912b9f32249ed63))
* **controls:** stop ZoomMiniMap stacking size handlers and throwing on clicks ([549ab13](https://github.com/atc-net/atc-wpf/commit/549ab13ebcb82ebfbb7d4a03a6ddcdbb2873efdd))
* **core:** add a Color overload of HsvColorUtil.ConvertRgbToHsv ([87d940c](https://github.com/atc-net/atc-wpf/commit/87d940c36d033cd52f1ae466aac6f9dc783ecd2d))
* **core:** compose SVG transform lists in SVG order ([9e61b4d](https://github.com/atc-net/atc-wpf/commit/9e61b4d340f7415bba9722247b854d6126ce34ea))
* **core:** deny unsafe drops and reorder observable collections with Move ([52486a0](https://github.com/atc-net/atc-wpf/commit/52486a0ef20a1b6c2171d1efbc37d1a3ec139b3c))
* **core:** detect double-clicks per button from ClickCount ([7f8bea1](https://github.com/atc-net/atc-wpf/commit/7f8bea1ccfe414a96b304004b0de6b05b803e217))
* **core:** do not rebuild the previous UI culture from its LCID ([8a512e6](https://github.com/atc-net/atc-wpf/commit/8a512e6e365f05631641407c91a30e3da01690c5))
* **core:** forget the pending drag start when a drag handler cancels ([ab3376c](https://github.com/atc-net/atc-wpf/commit/ab3376cb17495296f682714db2daca2806eea6cf))
* **core:** give the last panel child the LastItemMargin instead of the item margin ([ebc6351](https://github.com/atc-net/atc-wpf/commit/ebc6351cc27f7f2b6d4ac052ac02b5b5de1b6b1e))
* **core:** hook up TextBoxHelper monitoring when the element is loaded ([3fda461](https://github.com/atc-net/atc-wpf/commit/3fda4614be0c0417ec2c4a5a3a1dc4930ca1aaa2))
* **core:** include the orientation in PanelUvSize equality ([bc9f113](https://github.com/atc-net/atc-wpf/commit/bc9f113ab59dc9be0680717fbdaa5da4d118e71a))
* **core:** iterate a snapshot of the active markup extensions when updating targets ([e2f6067](https://github.com/atc-net/atc-wpf/commit/e2f606786897f4b0dc46ed0a14eb0f5950d20f2e))
* **core:** keep a pending hotkey chord when a bare modifier key is pressed ([fc22f95](https://github.com/atc-net/atc-wpf/commit/fc22f95b736d4aeb46e3ff33dbd1f17d7d46abc5))
* **core:** keep async navigation on the calling thread ([f602621](https://github.com/atc-net/atc-wpf/commit/f602621a70baf7c1e2c39012573755e3dc91b89e))
* **core:** keep the alpha channel when inverting bitmap colors ([0b21501](https://github.com/atc-net/atc-wpf/commit/0b2150124feb35e2e1155409a36ba769b503a2cd))
* **core:** keep the BoundPassword binding alive and always hook PasswordChanged ([fb9f4e7](https://github.com/atc-net/atc-wpf/commit/fb9f4e726301d23d5cce45e92d450615dedcc618))
* **core:** let a ResxExtension.Resource handler supply the resource value ([7f3954a](https://github.com/atc-net/atc-wpf/commit/7f3954a024b1da70682f0752cbb36b0c2f1bb31d))
* **core:** make EnumToArrayBindingSource work for nullable enum types ([751fcb6](https://github.com/atc-net/atc-wpf/commit/751fcb62b24138408717a7ecf451f213e1530d0e))
* **core:** marshal clipboard calls to the dispatcher and record history once ([8a570bb](https://github.com/atc-net/atc-wpf/commit/8a570bbd6836ccfcbf70a5b7bd7e943ed256a79f))
* **core:** mirror TextBlockHelper.Inlines changes by index and detach old collections ([877dea7](https://github.com/atc-net/atc-wpf/commit/877dea7cf82ad63f045bd0720fb39aeb09a8a085))
* **core:** monitor text changes on TextBox, RichTextBox and ComboBox ([6938194](https://github.com/atc-net/atc-wpf/commit/69381940b9732eb332a005d9f3598f03a9311a8b))
* **core:** parse GridEx row and column lengths independent of the culture ([91104e4](https://github.com/atc-net/atc-wpf/commit/91104e49015dc922118bdf346746e3e592b257f8))
* **core:** parse the BoolToWidth converter parameter independently of the culture ([9d1b5d7](https://github.com/atc-net/atc-wpf/commit/9d1b5d719ee805809dce6abdd5b0bfe4c329a3f3))
* **core:** push the default ToColor to the fade shader ([355f7a2](https://github.com/atc-net/atc-wpf/commit/355f7a2d4063de5d843d54c7216ca3dffdfe96c6))
* **core:** raise one indexed collection notification per change in the observable collections ([f5b953e](https://github.com/atc-net/atc-wpf/commit/f5b953ed3b9cd425ed88f09f645bc16192b5ec4c))
* **core:** render JSON nulls, return plain text and freeze the JSON brushes ([9b27cb6](https://github.com/atc-net/atc-wpf/commit/9b27cb6c8a3456b5a78bc94e94ebee4270bd3226))
* **core:** return null from ColorHelper.GetColorFromString for unknown color names ([dfab0a6](https://github.com/atc-net/atc-wpf/commit/dfab0a6d5654d1881ec3eadc931863d81e46ac9d))
* **core:** return the converted entries for Guid-keyed observable dictionaries ([41e1aaf](https://github.com/atc-net/atc-wpf/commit/41e1aaf5324def11f1f44bbd1377d51794ed4133))
* **core:** round instead of truncate when converting HSV to RGB ([52498e2](https://github.com/atc-net/atc-wpf/commit/52498e20ffd97c0d6435abe09a02f8cd113d5edb))
* **core:** stop AnimateAttach from leaking elements and stacking handlers ([3c26c1b](https://github.com/atc-net/atc-wpf/commit/3c26c1bdfff439be86cb90eea606b6d8dfe6b5ad))
* **core:** update markup extension targets on the thread that owns them ([43bbedf](https://github.com/atc-net/atc-wpf/commit/43bbedfc469c0681fa9fae847209f127c8a5ce8a))
* **dialogs:** own the dialog boxes by the window that opens them ([abfe6e2](https://github.com/atc-net/atc-wpf/commit/abfe6e27c8b955cb29982fd58a3223f9f3e14adb))
* **fonticons:** accept the converter parameter as a string for brush and em-size ([9c4f26d](https://github.com/atc-net/atc-wpf/commit/9c4f26d8e6ecf1bc83570327d82ca08b2c0d1cc3))
* **forms:** accept tcp and udp endpoints and require a mandatory endpoint host ([852eac2](https://github.com/atc-net/atc-wpf/commit/852eac265d5d7ec18329180e0ca9fb0a0a1157cf))
* **forms:** add IsValid to the date and time pickers ([e1e5bcc](https://github.com/atc-net/atc-wpf/commit/e1e5bcc2ae19829aaf6fd1e0b64a1eaa6864f482))
* **forms:** convert decimal control data values instead of unboxing them as int ([c247d02](https://github.com/atc-net/atc-wpf/commit/c247d022b4b6330e09d957a357b70c97ae1338d2))
* **forms:** do not raise LostFocusValid from the picker IsValid calls ([7fa9788](https://github.com/atc-net/atc-wpf/commit/7fa9788bc5e41f012232f7db71c9443587461f66))
* **forms:** format the selected date with the picker culture ([bddd476](https://github.com/atc-net/atc-wpf/commit/bddd47617b0b3f1b1f8a6fb2d5647230ca2aa881))
* **forms:** make IsValid on the decimal and currency boxes validate the current value ([5115d01](https://github.com/atc-net/atc-wpf/commit/5115d0171e10510addf25d1b4787b79d9f1e21b2))
* **forms:** raise IsOnChanged with the old and new values in the right order ([9e0f885](https://github.com/atc-net/atc-wpf/commit/9e0f8856f8931c16886e680307f72ea89fc88284))
* **forms:** read Range attribute bounds safely and keep the pixel size box limits ([b92be6b](https://github.com/atc-net/atc-wpf/commit/b92be6bc4e300004738393ff182e6464764ecc24))
* **forms:** report the edited text in the LabelTextBox lost-focus events ([38d52b0](https://github.com/atc-net/atc-wpf/commit/38d52b06f03833286f3f3d87ee03770c265edcc9))
* **forms:** skip collections and cap nesting depth in ModelToLabelControlExtractor ([be6c432](https://github.com/atc-net/atc-wpf/commit/be6c4326806f7e6ece9fd6e6ddbc3ed442803413))
* **hardware:** apply RefreshAsync results on the UI thread ([477b7a9](https://github.com/atc-net/atc-wpf/commit/477b7a9365e720b7e2f1968798a4c2b90abfb160))
* **hardware:** raise DeviceLost and rebind on in-place state changes ([b86ff95](https://github.com/atc-net/atc-wpf/commit/b86ff9586b5e8d61c8d41db92b732b605c17fd57))
* **hardware:** report the real Bluetooth connection state ([02a5ae7](https://github.com/atc-net/atc-wpf/commit/02a5ae7e30430563c49d9bc75a273c1e8ff9c1ce))
* **network:** keep the scanner and VNC viewer usable after failures ([c40330c](https://github.com/atc-net/atc-wpf/commit/c40330c32e7de231f3ca78995dc004a8a97b4122))
* **notifications:** own and close the desktop toast overlay with the application ([4a199a3](https://github.com/atc-net/atc-wpf/commit/4a199a3e857fd4bb92db45be1cc90ab59496ed30))
* **notifications:** register toast areas while loaded and stop sharing one content instance ([6b87c0b](https://github.com/atc-net/atc-wpf/commit/6b87c0ba18af96e8be30439662b3018482a54b4a))
* **printing:** stop swapping the printable area a second time for landscape ([978220b](https://github.com/atc-net/atc-wpf/commit/978220bd4a612550415915993827b644f0126271))
* **sample:** add a Close button to the borderless NiceWindow sample ([d05221a](https://github.com/atc-net/atc-wpf/commit/d05221a5bfa4b755d4bc0d47b693340a5ba38604))
* **sample:** judge markdown code-block colours by the editor background ([daed088](https://github.com/atc-net/atc-wpf/commit/daed088969281787d699fbc79d15b7ee131e6693))
* **sample:** show the readme tab when requested and clear it with the sample ([ac14999](https://github.com/atc-net/atc-wpf/commit/ac1499959e31d6bc645186d65ea23be5fa2042be))
* **svg:** accept comma and whitespace separated viewBox values ([78ec2b5](https://github.com/atc-net/atc-wpf/commit/78ec2b5cce19ff855315846e9db72d47cc3230c9))
* **svg:** apply clearing OverrideStrokeWidth on SvgImage ([94d8981](https://github.com/atc-net/atc-wpf/commit/94d89814d8aded38f47cc62d6191041e30b13653))
* **svg:** build clip rectangles from x/y and tolerate empty clip paths and poly shapes ([ad379f1](https://github.com/atc-net/atc-wpf/commit/ad379f10b865e387cb87f808240646809e0d409a))
* **svg:** default a gradient stop without a readable stop-color to black ([d987689](https://github.com/atc-net/atc-wpf/commit/d9876895e88acfa382806be5d98c7d682b0edfc9))
* **svg:** ignore an unreadable animation duration instead of throwing ([a68bec4](https://github.com/atc-net/atc-wpf/commit/a68bec4cc068f675a3e87c7d4b26eed73000fae4))
* **svg:** keep the radial gradient focal point unscaled ([81109f6](https://github.com/atc-net/atc-wpf/commit/81109f6e78564307b451ecbfdaddb9f750a37d26))
* **svg:** render rgb() and rgba() colours and apply fill-opacity per shape ([6c19345](https://github.com/atc-net/atc-wpf/commit/6c1934532cad0240bbb596863bac93952e32770c))
* **svg:** stop a use element that points at itself or an ancestor from recursing forever ([b3b1a25](https://github.com/atc-net/atc-wpf/commit/b3b1a25a78741a300530e7145ee63d838ae4cbce))
* **theming:** centre the NiceWindow title between the window commands ([8fec072](https://github.com/atc-net/atc-wpf/commit/8fec072cf0b750a326cd424817560c1d97e290a6))
* **theming:** define the resource keys used by the Calendar and DataGrid styles ([2ccf234](https://github.com/atc-net/atc-wpf/commit/2ccf234b0414411958e787e010f392b0f63c5ea0))
* **theming:** move the saved window rectangle into work-area coordinates ([4052fbb](https://github.com/atc-net/atc-wpf/commit/4052fbb1298d0911b06d9e2f0e48b212b628c50a))
* **theming:** resolve the parent window of WindowButtonCommands when it is loaded ([f4183d1](https://github.com/atc-net/atc-wpf/commit/f4183d1cfbf403c651ff2b6f1566143f5ded68eb))
* **theming:** return the documented fallbacks when no theme is detected ([e414fb8](https://github.com/atc-net/atc-wpf/commit/e414fb858dec740215cc8b7106ed55ec7e7a315d))
* **theming:** skip a saved window placement that is off every screen ([f4a0971](https://github.com/atc-net/atc-wpf/commit/f4a09710cde022be5ce6db7365700fe0ecdfe05d))
* **theming:** stop NiceWindow from resetting the process culture on render ([50b12ff](https://github.com/atc-net/atc-wpf/commit/50b12ff2f6c60fda22685236746e0ce3fc484606))
* **theming:** upgrade the window placement settings after a version change ([1b9b436](https://github.com/atc-net/atc-wpf/commit/1b9b4367e7f9b9185e15784da886d986564fd3b4))
* **viewers:** keep ANSI escape codes out of TerminalViewer line text ([41fb697](https://github.com/atc-net/atc-wpf/commit/41fb697ecb4a8439c4a9d19f193c440db3f041e7))

## [4.1.0](https://github.com/atc-net/atc-wpf/compare/v4.0.215...v4.1.0) (2026-10-09)


### New features

* **components:** add TrayIcon notification area icon ([fb1f7a9](https://github.com/atc-net/atc-wpf/commit/fb1f7a96d9556796ec0abd8ae0fdabde7178a078))
* **components:** address terminal output to a specific TerminalViewer ([ca2670f](https://github.com/atc-net/atc-wpf/commit/ca2670fd90a900c7b2509de7934cf34e765c1b80))
* **components:** render the xterm 256-colour palette in TerminalViewer ([d058b80](https://github.com/atc-net/atc-wpf/commit/d058b80bd00958931561fdd456f1661d4e45eedb))
* **controls:** add NavigationView shell bound to INavigationService ([78549fa](https://github.com/atc-net/atc-wpf/commit/78549fa3aba433ea8010d25839930887e6db7e23))
* **forms:** forward UpdateUiCultureOnChangeEvent from LabelCountrySelector ([547188b](https://github.com/atc-net/atc-wpf/commit/547188b4543c0530ff31ac17762fb2abf70e99c3))
* **hardware:** share picker behaviour and expose service tuning ([5187d15](https://github.com/atc-net/atc-wpf/commit/5187d15fdae772d9c52b8c2a59d662d0831fdc38))
* **hotkeys:** report global hotkeys that Windows refuses to register ([8cdd071](https://github.com/atc-net/atc-wpf/commit/8cdd071b414115972accbfc3b75955b322e0de0a))
* improve NetworkValidationRule and EndpointBox to handle hostname ([1336d80](https://github.com/atc-net/atc-wpf/commit/1336d803c8b9ce8f13db453113c69a7d11d2fc62))
* **sample:** add a right-to-left switch to the sample viewer ([a9a11a8](https://github.com/atc-net/atc-wpf/commit/a9a11a86f102b8ed95a2d3921adefbff63d4f661))
* **theming:** add Windows 11 backdrops to NiceWindow and keep it intact under ThemeMode ([a3260c5](https://github.com/atc-net/atc-wpf/commit/a3260c565bc575a9c630ef31fb11018d770c2775))
* **theming:** follow the Windows app mode and accent color ([f04c488](https://github.com/atc-net/atc-wpf/commit/f04c488324a02094004f4c6169bf1d35a4399136))
* **theming:** follow Windows high contrast ([2c8e03b](https://github.com/atc-net/atc-wpf/commit/2c8e03b337f712dcdc71b745aa20cedfced0d725))
* **zoom:** make ZoomGridOverlay styling bindable and release it on Detach ([05be68b](https://github.com/atc-net/atc-wpf/commit/05be68bdd3bbc01c0ca820ce60db8beaea6b52cf))


### Bug fixes

* **components:** animate a flyout the first time it opens ([7889a86](https://github.com/atc-net/atc-wpf/commit/7889a86519756ef84f8504c51792c7cd15be67c9))
* **components:** close the oldest toasts when MaxItems is exceeded ([6bea803](https://github.com/atc-net/atc-wpf/commit/6bea8036d9d4c258bfa1799c767f086876e479f2))
* **components:** keep JSON and terminal content left-to-right in right-to-left layouts ([9845157](https://github.com/atc-net/atc-wpf/commit/984515791746a1f5721b88f5edd3afcfa56d5ab6))
* **components:** let clicks pass through an empty FlyoutHost ([38bf8d3](https://github.com/atc-net/atc-wpf/commit/38bf8d3e31094c2034875bbd8ad3cf408945066d))
* **components:** open print preview and desktop toasts in the app's flow direction ([5e65216](https://github.com/atc-net/atc-wpf/commit/5e65216d13e6b9222a150c643a187b1bf399ccbc))
* **components:** stop CloseAllFlyouts hanging during close animations ([0560b7f](https://github.com/atc-net/atc-wpf/commit/0560b7f16d5e2d192898ca5722c6043d724b7943))
* **components:** strip private-mode ANSI sequences and truncated colours ([ae6277f](https://github.com/atc-net/atc-wpf/commit/ae6277f59a45ff4605628679c89bca8d1777a154))
* **controls:** accept lower-case percent and hex letters in NumericBox ([099a07d](https://github.com/atc-net/atc-wpf/commit/099a07daf4f51dc6067a5ac2489f59d6fe334511))
* **controls:** align UniformSpacingPanel measure with arrange ([ba63f0f](https://github.com/atc-net/atc-wpf/commit/ba63f0fc367a67cee720d6b4f1014d8a28834ee4))
* **controls:** apply ZoomMiniMap viewport border properties ([ac90a34](https://github.com/atc-net/atc-wpf/commit/ac90a34a494a498bb313d6183bba598703032aac))
* **controls:** change the UI culture from CountrySelector when enabled ([b78526c](https://github.com/atc-net/atc-wpf/commit/b78526cf3ca9265984683c605193a340f31de909))
* **controls:** keep country and language flags unmirrored in right-to-left ([2717633](https://github.com/atc-net/atc-wpf/commit/2717633fd625641687ef0386828f2c88734a6095))
* **controls:** keep ZoomBox content unmirrored when it opens in right-to-left ([0d5b05a](https://github.com/atc-net/atc-wpf/commit/0d5b05ad7955386b552d5699c80395eeb162192f))
* **controls:** keep ZoomRuler labels readable in right-to-left ([6bdd4eb](https://github.com/atc-net/atc-wpf/commit/6bdd4eb335d4b641c6f11fecaea4851dc1204496))
* **controls:** let FilePicker and DirectoryPicker inherit the DataContext ([64fac3c](https://github.com/atc-net/atc-wpf/commit/64fac3c230e527a4380ee37a33b91da4d83660af))
* **controls:** measure VirtualizingStaggeredPanel without infinite sizes ([7e21264](https://github.com/atc-net/atc-wpf/commit/7e21264a880295d56839207612026dbf0a54508d))
* **controls:** mirror the ZoomMiniMap thumbnail for right-to-left content ([b47b2d1](https://github.com/atc-net/atc-wpf/commit/b47b2d174e6c34809af4316397b4550ec9094506))
* **controls:** raise the number box LostFocus events once per edit ([fd3820d](https://github.com/atc-net/atc-wpf/commit/fd3820d437fb95c6fa0e8fa31b754a466e6aad67))
* **controls:** re-arrange ReversibleStackPanel when ReverseOrder changes ([27a81b7](https://github.com/atc-net/atc-wpf/commit/27a81b71e2fae8faf2d046972e0026ba780a45e7))
* **controls:** share column layout between StaggeredPanel measure and arrange ([8a682ba](https://github.com/atc-net/atc-wpf/commit/8a682bad31efcf27bb189a593ead8ce42bb08779))
* **controls:** stop theme-change subscriptions from leaking controls ([a3b357a](https://github.com/atc-net/atc-wpf/commit/a3b357a2d292cb2b29e414cf7911e279ff221448))
* **controls:** swap arrow keys in right-to-left layouts ([1ccbf89](https://github.com/atc-net/atc-wpf/commit/1ccbf8904649593a87c7c3e77f717eb610276173))
* **dialogs:** correct the DialogBoxSettings placeholder texts and docs ([5b95315](https://github.com/atc-net/atc-wpf/commit/5b95315d3a3d3f291f13c748ff749d2072ac0ccd))
* **dialogs:** follow the owning window's flow direction ([e326e4f](https://github.com/atc-net/atc-wpf/commit/e326e4fbe949307450ccca5a2109a0e027812f51))
* **dialogs:** make DialogService cancellation close the open dialog ([7c04c15](https://github.com/atc-net/atc-wpf/commit/7c04c15bd1b938f4139f7d31f9a8e129239eadd5))
* **forms:** declare the combo box selector events on ILabelComboBoxBase ([8ba9f64](https://github.com/atc-net/atc-wpf/commit/8ba9f6495ab6b27472750cce6e9ace1f4a58c33a))
* **forms:** make LabelSlider.IsValid validate the current value ([b5ac102](https://github.com/atc-net/atc-wpf/commit/b5ac102c56f0d883cf37916ec5ade074848311a2))
* **forms:** raise EndpointBox LostFocus events when an editor loses focus ([aec88c5](https://github.com/atc-net/atc-wpf/commit/aec88c51fa0bc10d64fdde696d328cf8a2853421))
* **forms:** raise TextChanged from LabelDatePicker and LabelTimePicker ([1c2163a](https://github.com/atc-net/atc-wpf/commit/1c2163a14ea0ae6850133c79cb3d7bf8bfcfe754))
* **forms:** report the previous color from ColorPicker.ColorChanged ([996ad7a](https://github.com/atc-net/atc-wpf/commit/996ad7a76eb99198e19732f077da8c955ebe88e3))
* **hardware:** keep unplugged devices disconnected after the just-connected delay ([71fca55](https://github.com/atc-net/atc-wpf/commit/71fca55c83705ce6d722b057b1ca983bcca21c68))
* **hardware:** refresh display and network adapter details while connected ([4de7a43](https://github.com/atc-net/atc-wpf/commit/4de7a4372a91e78022fc198332efac818cc91d31))
* **hardware:** refresh polled process, window, drive and printer entries ([c9f919c](https://github.com/atc-net/atc-wpf/commit/c9f919c0a2dbb84bba7260cc2d1b85a7fd2176bf))
* **hardware:** stop the camera and microphone running after unload ([96dcd9b](https://github.com/atc-net/atc-wpf/commit/96dcd9b2d4059fc670da135a76e1061cc614b98d))
* keep the original exception as inner exception in RtfFormatter and ColorPickerAutomationPeer ([fff08d4](https://github.com/atc-net/atc-wpf/commit/fff08d4058e7b9881a01e3b6f125d02de251bc30))
* **media:** keep SVG external file references inside the SVG folder ([6b76bc8](https://github.com/atc-net/atc-wpf/commit/6b76bc89c87a8870b2104c34d93edfa49614a1e5))
* **media:** make the AutoGrey extensions work and keep transparency ([4b75dfa](https://github.com/atc-net/atc-wpf/commit/4b75dfa699d53c2d5b8a303e19399237b18023ac))
* **media:** render an SvgImage once when it loads and keep custom brushes ([d3c1311](https://github.com/atc-net/atc-wpf/commit/d3c1311c5cce9c19b552d5ea4ae7c09938203741))
* **network:** report a dropped VNC socket as a disconnect instead of crashing ([3af02c0](https://github.com/atc-net/atc-wpf/commit/3af02c0a41505f2db13d5d00358929a4e5f2d4a1))
* **notifications:** name the toast factory title parameter title ([ad6cda9](https://github.com/atc-net/atc-wpf/commit/ad6cda98c00110ad2d3871f5d596c559853ce743))
* **sample:** match readme files from a folder boundary in the sample viewer ([04c56d2](https://github.com/atc-net/atc-wpf/commit/04c56d29b8c6b49baef278c893d63c83f122b565))
* suppress some coding rules and fix code ([71f6465](https://github.com/atc-net/atc-wpf/commit/71f646551bad7ce294223f07341960cd589cdd0c))
* **theming:** keep the DatePicker calendar icon unmirrored in right-to-left ([9666f3f](https://github.com/atc-net/atc-wpf/commit/9666f3f328dd5cd1e3039102b085f49b252132cc))
* **translation:** notify culture subscribers on their own UI thread ([1496ed9](https://github.com/atc-net/atc-wpf/commit/1496ed96701ac20c9738f2023d5c6ce3d463e6c8))
* **translation:** skip sealed style setters when updating translated values ([2074c09](https://github.com/atc-net/atc-wpf/commit/2074c09abeb46fdb904eb090d3ff1a6ca08ed735))
* **viewers:** use the JsonPropertyTemplateSelector template properties ([5cb3496](https://github.com/atc-net/atc-wpf/commit/5cb3496f8ddb7963ca2b35e30635c2da69851e5d))
* **zoom:** reject non-positive or non-finite grid spacing ([4c22453](https://github.com/atc-net/atc-wpf/commit/4c224537b2e61bb9f3818bbdaa6e387a9253d214))


### Performance improvements

* **hardware:** enumerate processes and windows off the UI thread ([dbb50a3](https://github.com/atc-net/atc-wpf/commit/dbb50a393f467861ea45ff572bd81419657d59dd))
* **hardware:** query the print spooler off the UI thread ([7bccafc](https://github.com/atc-net/atc-wpf/commit/7bccafc67a87046eaf1673b080fa61ea4d745ea4))
* **hardware:** reuse camera preview frame buffers and coalesce renders ([7f9575b](https://github.com/atc-net/atc-wpf/commit/7f9575b7bba4c47ef003332ab499eced99cc760e))
* **media:** cache AutoGreyableImage greyscale versions per source ([eedc3eb](https://github.com/atc-net/atc-wpf/commit/eedc3eb35f4fc4a9252f901f1c4f16686d837303))
* **media:** share frozen SVG drawings between images with the same source ([b702644](https://github.com/atc-net/atc-wpf/commit/b70264445d72a9a74692593294f63c81b717a718))
* **network:** stop refreshing the whole scanner view on every progress report ([c2d14ad](https://github.com/atc-net/atc-wpf/commit/c2d14adce1af70d12a4e2525fca9fe69c9492822))
* **zoom:** reuse pens and text resources in ZoomRuler and ZoomGridOverlay ([67af5d3](https://github.com/atc-net/atc-wpf/commit/67af5d36d8fc1ac8b6d404ef810fdb022b113321))

## 4.0.215 (2026-05-12)

### Added

- **`ApplicationMonitorView` upgrades** in `Atc.Wpf.Components.Monitoring`:
  - **Virtualization** — `VirtualizingPanel.IsVirtualizing="True"` with
    `VirtualizationMode="Recycling"` and pixel scroll. Tens of thousands of
    rows scroll smoothly.
  - **Channel-based batched ingestion** — entries flow through an unbounded
    `Channel<ApplicationEventEntry>`; a background drain loop dispatches
    batches to the UI thread under a single `InvokeAsync(Background)` and
    sends one `ApplicationMonitorScrollEvent` per batch. The TerminalViewer
    pattern, applied to the monitor.
  - **`MaxEntries`** DP (default `10000`, `0` = unbounded) — ring-buffer
    cap; oldest entries (by insertion order) drop when exceeded. Two-way
    bindable, bridged to the VM.
  - **`IsPaused`** DP + `ShowPauseInToolbar` DP — Pause/Resume toolbar
    toggle freezes display while the channel keeps capturing; the button
    shows a badge with the buffered-but-not-shown count
    (`BufferedCount` on the VM, exposed via `ChannelReader.Count`).
  - **Smart auto-scroll ("tail mode")** — `ScrollChanged` hook detects
    when the user moves away from the tail; auto-scroll suppresses and a
    floating **"↓ Jump to live (N)"** overlay appears with a count of new
    entries since detachment. Click (or call `JumpToLive()` from code) to
    re-attach. New `IsDetachedFromTail` and `NewSinceDetached` DPs back
    the overlay binding. Direction-aware (works for ascending and
    descending sort).
  - **`ExportCommand`** + `ShowExportInToolbar` DP — toolbar Export button
    opens a `SaveFileDialog` (CSV / JSON / TXT) and writes the currently
    *visible* (filtered) entries via the new internal
    `ApplicationMonitorExportService`. Format inferred from the chosen
    extension; error path surfaces a `MessageBox`.
  - **`ApplicationMonitorLoggerProvider`** in
    `Atc.Wpf.Components.Monitoring.Logging` — drop-in
    `Microsoft.Extensions.Logging` provider. `builder.Logging.AddAtcWpfApplicationMonitor()`
    routes every `ILogger<T>` call into the live picker via `Messenger.Default`
    (`LogLevel` → `LogCategoryType` mapping, `categoryName` → `Area`).
    Optional level filter overload. `ProviderAlias("AtcWpfApplicationMonitor")`
    so it can be addressed by name from `appsettings.json` filters.

- **`AudioInputPicker.ShowLivePreview`** + **`AudioOutputPicker.ShowLivePreview`**
  with a shared `PreviewHeight` DP. When enabled and a device is selected,
  the picker shows a small live pane below the dropdown — a scrolling
  waveform + side peak-level bar. Both backed by WinRT `AudioGraph` (no
  NAudio dependency).

  `AudioInputPicker` opens the selected microphone via
  `AudioDeviceInputNode` + `AudioFrameOutputNode`; per-quantum peaks
  are pushed into a 200-slot ring buffer rendered at ~30 fps as
  mirrored top/bottom polylines. Permission denial and "device held
  by another app" surface as localized inline messages
  (`MicrophonePermissionDenied` / `AudioPreviewUnavailable`).

  `AudioOutputPicker` adds a `Test` / `Stop` button next to the
  waveform: clicking `Test` opens the selected speakers via
  `AudioGraph.PrimaryRenderDevice` + `AudioFrameInputNode` and feeds
  a 1 kHz sine for ~3 s — Left → Right → Both stereo, with a 30 ms
  linear fade in/out per segment to suppress click artefacts. The
  same buffer drives the visualisation, so the picker shows exactly
  what's being rendered. The fade-envelope math lives in
  `SineToneEnvelope` so it stays unit-tested independently of the
  `AudioGraph` runtime.

  Both DPs are forwarded through `LabelAudioInputPicker` and
  `LabelAudioOutputPicker`. `Test`, `Stop`, `AudioPreviewUnavailable`,
  and `AudioPermissionDenied` localised across en-US / da-DK / de-DE.

  Buffer access uses the WinRT `IMemoryBufferByteAccess` COM interface
  (the only safe path to the raw audio bytes — `AudioBuffer` has no
  `CopyToBuffer(IBuffer)` overload like `SoftwareBitmap` does), so
  `<AllowUnsafeBlocks>` is now enabled at the project level. `unsafe`
  is confined to `IMemoryBufferByteAccess.cs` and `AudioBufferAccess.cs`;
  the rest of the assembly stays in safe code.

  Closes the §9 audio live preview roadmap. 8 new unit tests for the
  sine-tone envelope (segment boundaries, fade-in/out linearity,
  past-end silence, edge cases).
- **`UsbCameraPicker.PreferredFormat`** + lazy supported-format
  enumeration. `UsbCameraFormat` is a new record (Width / Height /
  FrameRate / Subtype) and `UsbCameraInfo.SupportedFormats` is an
  `[ObservableProperty]` populated lazily once `LiveCameraPreview`
  opens the camera and reads `MediaFrameSource.SupportedFormats`. The
  picker subscribes to a new `LiveCameraPreview.FormatsAvailable`
  event and forwards the list (sorted descending by resolution then
  FPS, deduplicated) onto the bound `UsbCameraInfo`. The
  `PreferredFormat` DP is a culture-invariant hint: when set, the
  preview calls `MediaFrameSource.SetFormatAsync` for the matching
  format before starting the reader; if no exact match, the preview
  falls back silently to the device default. Forwarded through
  `LabelUsbCameraPicker`. Sample app shows a `ComboBox` bound to
  `Value.SupportedFormats` ↔ `PreferredFormat` to demonstrate the
  end-to-end flow. Closes the §3.1 / §3.2 v2 items in the picker
  roadmap.
- **`UsbCameraPicker.ShowLivePreview`** + companion `PreviewHeight` DP.
  When `ShowLivePreview="True"` and a camera is selected, the picker
  renders a live preview pane below the dropdown driven by an internal
  `LiveCameraPreview` control. The preview opens the camera via WinRT
  `MediaCapture` + `MediaFrameReader`, normalises each frame to BGRA8
  premultiplied, and copies pixel bytes into a recycled `WriteableBitmap`
  on the UI thread (no `AllowUnsafeBlocks`, no per-frame BMP encode).
  The first activation triggers Windows' webcam-permission prompt; if
  the user denies access or another process holds the device, the pane
  shows a localized inline message (`PreviewPermissionDenied` /
  `PreviewUnavailable`) rather than crashing the picker. Preview
  lifecycle restarts on `Value` changes and stops cleanly on `Unloaded`
  / `ShowLivePreview="False"` / `Dispose`. Forwarded through
  `LabelUsbCameraPicker`. Strings localised for `en-US` / `da-DK` /
  `de-DE`. The separate `PreferredFormat` DP for picking a specific
  resolution/FPS remains parked v2.
- **Runtime culture smoke tests** for `Atc.Wpf.Hardware` resources.
  Flips `Miscellaneous.Culture` and `Validations.Culture` between
  invariant / `da-DK` / `de-DE` and asserts representative keys come
  back in the right language; 34 cases across 11 keys (8 picker /
  state strings in `Miscellaneous`, 3 messages in `Validations`).
  Tests use xUnit `[Collection("Localization")]` so the static
  resource-culture flips don't race in parallel runs. Retires §5 last
  ⬜ deferral on the picker roadmap.
- **WinRT-free `IDeviceWatcherHost` abstraction + `FakeDeviceWatcherHost`
  test harness** for `Atc.Wpf.Hardware`. The concrete `DeviceWatcherHost`
  now implements `IDeviceWatcherHost` and maps WinRT `DeviceInformation`
  to a `DeviceSnapshot` POCO before raising `Added` / `Updated` /
  `Removed` / `EnumerationCompleted`. Each WinRT-backed service
  (Serial / USB / UsbCamera / Audio / Bluetooth) accepts the host (or a
  `Func<string, IDeviceWatcherHost>` factory for `UsbDeviceService` to
  support its filter-rebuild flow) via an internal constructor. The
  test project ships a `FakeDeviceWatcherHost` that drives event
  scenarios deterministically — retires the four "deferred — needs
  WinRT mock harness" items in the roadmap (§1.5, §2.5, §3.5, §4.8).
  Adds 34 service tests (95 total in `Atc.Wpf.Hardware.Tests`, up from
  61) covering Added / Removed / RefreshAsync / Dispose / idempotent
  Start/Stop / Available↔Disconnected rebinding / IsEnabled→InUse
  mapping. No public API change.
- **`DisplayPicker`** in `Atc.Wpf.Hardware` — picker for connected
  monitors/displays. Uses Win32 `EnumDisplayMonitors` + `GetMonitorInfo`
  (P/Invoked from user32.dll, with the same DllImport pattern as
  WindowPicker) to capture each monitor's `DeviceName`, full
  `Bounds`, `WorkingArea`, and the system-primary flag. Polls every
  2 s for hot-plug. The system primary monitor is rendered with a ★
  followed by its resolution (e.g. `\\.\DISPLAY1 ★ (1920×1080)`).
  Strings localised for `en-US` / `da-DK` / `de-DE`. Four new model
  tests cover constructor, primary/non-primary `ToString` formatting,
  and INPC.
- **`NetworkAdapterPicker` / `PrinterPicker`** in `Atc.Wpf.Hardware` —
  two more polling-based system pickers. `NetworkAdapterPicker`
  enumerates `NetworkInterface.GetAllNetworkInterfaces()` (with
  `IncludeLoopback=false` by default to hide loopback adapters) and
  exposes a *reactive* `OperationalStatus` so consumers can bind live
  up/down state without re-selecting. `PrinterPicker` enumerates
  `LocalPrintServer.GetPrintQueues(Local | Connections)`, including
  the system default printer (rendered with a ★ in the dropdown) and
  the queue's `IsShared` / `IsLocal` / `QueueStatus`. Both poll every
  2 s and route through the same `DeviceState` plumbing as the rest
  of the family. Strings localised for `en-US` / `da-DK` / `de-DE`.
  Eight new model tests cover both POCOs.
- **`ProcessPicker` / `WindowPicker`** in `Atc.Wpf.Hardware` —
  inspection-style pickers for hooking debuggers, automation, capture
  targets, or "attach to existing" flows. `ProcessPicker` enumerates
  `Process.GetProcesses()` (with `OnlyWithMainWindow=true` by default to
  hide background services); `WindowPicker` enumerates top-level windows
  via `EnumWindows` P/Invoked from `user32.dll` (with
  `OnlyVisibleWithTitle=true` by default). Both poll every 2 s instead of
  using DeviceWatcher (no OS hot-plug API for these). Same `DeviceState`
  surface as the hardware pickers — when a tracked process exits or
  window is destroyed, `DeviceLost` fires and the bound `Value` flips to
  `Disconnected`. `RunningProcessInfo` and `TopLevelWindowInfo` POCOs
  expose typed metadata (PID, MainWindowTitle, MainModulePath, HWND,
  ClassName, owning process). Strings localised for `en-US` / `da-DK`
  / `de-DE`. Eight new model tests cover both POCOs.
- **`BluetoothDevicePicker`** in `Atc.Wpf.Hardware` — paired-classic
  Bluetooth picker built on the same `DeviceWatcherHost` plumbing as the
  serial/USB/audio pickers, using `BluetoothDevice.GetDeviceSelectorFromPairingState(true)`
  as the AQS selector. Surfaces `IsConnected` and `IsPaired` on the
  `BluetoothDeviceInfo` model and renders connected entries with a ●
  bullet in the dropdown. Strings localised for `en-US` / `da-DK` /
  `de-DE`. BLE-only and unpaired-discovery scenarios are deferred to v2
  (require additional capability declarations).
- **`DrivePicker`** in `Atc.Wpf.Hardware` and **`TimeZonePicker`** in
  `Atc.Wpf.Forms` — two pure-managed pickers. `DrivePicker` enumerates
  `System.IO.DriveInfo.GetDrives()` (Fixed / Removable / Network / CDRom /
  Ram) and polls every 2 s for hot-plug — same `DeviceState` UX, same
  `ValueChanged` / `DeviceLost` / `DeviceReconnected` / `DeviceStateChanged`
  routed events, same Label* wrapper pattern. `TimeZonePicker` is the slim
  variant — `TimeZoneInfo.GetSystemTimeZones()` bound directly, no service,
  no hot-plug, no state, just a clean ComboBox with offset + display name.
  Lives in `Atc.Wpf.Forms` (alongside the other labeled pickers) since it
  has no hardware involvement. `LabelTimeZonePicker` adds mandatory
  validation. `TimeZone` / `SelectTimeZone` strings live in the shared
  `Atc.Wpf.Controls.Resources.Miscellaneous` (`en-US` / `da-DK` / `de-DE`).
  New `DiskDriveInfo` model (named to avoid the clash with `System.IO.DriveInfo`).
- **`AudioInputPicker` / `AudioOutputPicker`** in `Atc.Wpf.Hardware` —
  microphone and speaker pickers built on the same `DeviceWatcherHost`
  plumbing as the existing pickers, using `DeviceClass.AudioCapture` and
  `DeviceClass.AudioRender`. Both surface live state (Available / InUse /
  Disconnected / JustConnected), raise `ValueChanged` / `DeviceLost` /
  `DeviceReconnected` / `DeviceStateChanged`, and ship labeled wrappers
  (`LabelAudioInputPicker`, `LabelAudioOutputPicker`) with mandatory +
  disconnected-device validation. The system default endpoint is rendered
  with a ★ in the dropdown. New `AudioDeviceInfo` model and `AudioDeviceKind`
  enum. Strings localised for `en-US` / `da-DK` / `de-DE`.
- **USB `ClassFilter` → AQS translation** in `Atc.Wpf.Hardware` —
  `UsbDeviceClassFilter` flags (Hid / Imaging / Audio / Printer / MassStorage
  / Communication) now translate to a real `System.Devices.InterfaceClassGuid`
  AQS query OR-joining the relevant device interface GUIDs, and the
  `DeviceWatcher` rebuilds when the filter changes. Previously the filter
  was stored but never applied — narrowing happens at the OS level now.
- **`Atc.Wpf.Hardware` package** — new assembly providing three hardware
  picker controls (`SerialPortPicker`, `UsbPortPicker`, `UsbCameraPicker`)
  and their labeled wrappers (`LabelSerialPortPicker`, `LabelUsbPortPicker`,
  `LabelUsbCameraPicker`) with **live device-state detection**. Connect /
  disconnect / in-use are reflected in the dropdown via colour-coded status
  dots (green / amber / red) — the user never has to click *Refresh* and
  never picks a device that's silently unusable. When a *bound* `Value`
  device disconnects, the picker raises `DeviceLost`, shows an inline
  warning, and preserves the selection so reconnect rebinds silently
  (`AutoRebindOnReconnect` default-on); auto-clear is opt-in via
  `ClearValueOnDisconnect`. Hot-plug detection is shared infrastructure
  via `Windows.Devices.Enumeration.DeviceWatcher` (no `WM_DEVICECHANGE`
  plumbing). User-visible strings localised for `en-US` / `da-DK` /
  `de-DE`. Targets `net10.0-windows10.0.19041.0` (Windows 10 May 2020 / 2004
  and later, all Windows 11). See `docs/Hardware/@Readme.md` and
  `docs/roadmap-pickers.md`.
- **`ColorEditorMode` on the FontPicker family** — new `FontColorEditorMode`
  enum (`WellKnownColorSelector` / `ColorPicker`) lets consumers pick between
  an inline named-color dropdown and the dialog-based color picker for the
  Foreground / Background fields inside `AdvancedFontPicker`. `LabelFontPicker`,
  `FontPicker`, and `FontPickerDialogBox` default to `WellKnownColorSelector`
  so opening a font picker no longer cascades into a *second* modal dialog when
  the user edits a colour; standalone `AdvancedFontPicker` keeps the dialog
  variant by default. Each level exposes the property as a DP so consumers can
  override (e.g. set `ColorEditorMode="ColorPicker"` if nested dialogs are
  desired). Backed by a new two-way `BrushToColorNameValueConverter` in
  `Atc.Wpf.ValueConverters` that maps between `SolidColorBrush` and the
  English-invariant well-known colour key. The hidden well-known selector uses
  `DropDownFirstItemType="Blank"` so theme-default brushes whose colour does
  not match any named entry don't get silently rewritten on load.
- **FontPicker control family** in `Atc.Wpf.Forms` — a four-tier font selection
  stack mirroring the existing ColorPicker trinity:
  - `FontPicker` (compact): `"Aa"` sample rendered in the selected font + family
    name + size in the host UI font + an edit button that opens the dialog.
  - `FontPickerDialogBox`: `NiceDialogBox` wrapper around `AdvancedFontPicker`
    with OK / Cancel and full TwoWay round-trip of the selection.
  - `AdvancedFontPicker`: full editor with a responsive star-column layout —
    family list with type-ahead search, recently-used items, virtualised
    all-fonts view; Weight / Style / Stretch lists derived dynamically from
    `family.FamilyTypefaces` (only valid combinations) with snap-to-closest
    when switching families; size as `IntegerBox` + preset list with custom
    sizes auto-inserted; Foreground / Background `LabelColorPicker`s with
    inline swatch; Bold / Italic / Underline / Strikethrough quick toggles
    bound bidirectionally; editable preview text + multi-sample preview area
    with theme-aware default colours and a WCAG contrast-warning glyph.
  - `LabelFontPicker`: form-tier wrapper mirroring `LabelColorPicker`.
  - `FontDescription` value type bundles all appearance properties (Family,
    Size, Weight, Style, Stretch, Foreground, Background, TextDecorations)
    with `ApplyTo` / `FromControl` helpers so a picker round-trips cleanly
    against any `Control`.
  - Granular `Show*` / `IsEnabled*` DPs per section, plus a 🔒 indicator on
    disabled `GroupBox`es. Full `AutomationProperties.Name` + ToolTips +
    keyboard navigation (Tab / arrow / type-ahead).
  - Recents persisted via the new `IFontPickerStorage` abstraction (default:
    in-memory, capped at 8); apps wanting cross-restart persistence assign
    their own implementation to `FontPickerStorage.Current`.
  - Translation keys added in `en` / `da` / `de` for `FontPicker`, `FontFamily`,
    `FontSize`, `FontWeight`, `FontStyle`, `FontStretch`, `Preview`,
    `Foreground`, `Background`, `Bold`, `Italic`, `Underline`, `Strikethrough`,
    `Locked`, `ContrastWarning`.
- **`ToggleButton` theme styles** in `Atc.Wpf.Theming`, mirroring the existing
  `Button.xaml` style hierarchy. `AtcApps.Styles.ToggleButton` (+ `.Small` /
  `.Large`) carries an `IsChecked` trigger that swaps Background / BorderBrush
  to the existing Pressed brushes; chromeless variants apply a subtle background
  on checked. Seven Bootstrap colours (Default / Primary / Secondary / Success /
  Danger / Warning / Info) ship in solid + outline flavours and three sizes —
  outline variants additionally swap Foreground to white when checked for
  legibility. The implicit `TargetType="ToggleButton"` style is registered in
  `Controls.xaml`, so any unstyled `ToggleButton` in a consuming app picks it up
  automatically. New "Theming → Input - Button → ToggleButton" sample page
  replaces a previously-disabled placeholder.
- **`ApplicationMonitorView` context menu** with Copy / Copy Selected / Copy All
  commands. New `EnableContextMenu` DP gates the menu; `[RelayCommand]` handlers
  carry `CanExecute` predicates so redundant items hide rather than grey out.
  The `ListView` switches to `SelectionMode="Extended"` with a `SelectionChanged`
  shim that bridges multi-selection into the VM. `CopyAllToClipboard` /
  `CopySelectedToClipboard` resource strings added in `en` / `da` / `de`.

### Fixed

- **`CultureManager.UiCultureChanged` is now a weak event.** Long-lived static
  subscriptions no longer root WPF controls, removing a systemic memory leak.
  Existing subscribers (`+= OnUiCultureChanged`) continue to work unchanged.
  Lambda subscribers with closures must keep the delegate referenced themselves,
  as documented on the event.
- **`TerminalViewer.Dispose` no longer risks a UI-thread deadlock.** The previous
  `Wait()` on a task that posts back to the dispatcher was replaced with a
  cancel + dispose-on-continuation pattern. The `Channel<>` writer is now
  completed during `Dispose`, and `Messenger.Default.UnRegister(this)` is called
  to release subscriptions promptly.
- **`Dispatcher.Invoke` paths in `TerminalViewer` short-circuit via `CheckAccess()`**,
  eliminating an unnecessary marshaling hop when already on the UI thread.
- **SVG `TextShape` parse failures are now logged** via `Trace.TraceError`
  instead of being silently swallowed, matching `ImageShape`.
- **`Messenger.Default.Register` calls are paired with `UnRegister`** in `ZoomBox`
  and `ApplicationMonitorView` (registration moved to `Loaded`/`Unloaded`),
  preventing dead controls from continuing to receive messages until the next
  `Cleanup()`.
- **`ObservableDictionary<TKey, TValue>.CopyTo` is now implemented** with proper
  argument validation; the type can now be passed to APIs expecting
  `ICollection<KeyValuePair<,>>`.
- **`RenderFlagIndicatorTypeToVisibilityValueConverter.ConvertBack` and
  `ZoomMiniMapClampMultiValueConverter.ConvertBack`** now throw
  `NotSupportedException` (matching the WPF idiom for one-way converters)
  instead of `NotImplementedException`.

### Performance

- **`SolidColorBrushHelper.GetBrushFromString`** is now O(1) via a reverse-lookup
  dictionary instead of an O(n) LINQ scan over the localized brush dictionary.
- **`VirtualizingStaggeredPanel`** reuses a `HashSet<int>` scratch buffer for
  visible-index tracking and replaces `columnHeights.Max()` with a manual loop,
  removing per-measure-pass allocations.
- **`GridEx.OnRowsChanged` / `OnColumnsChanged`** now share a single
  `GridLengthConverter` instead of allocating one per parse.
- **`BitmapImageFactory`** freezes absolute-URI `BitmapImage` results
  (`CacheOption.OnLoad` + `Freeze()`), making them safe to share across threads
  and avoiding repeated decode work.
- **`DebounceDispatcher`** reuses its `DispatcherTimer` across calls when the
  priority + dispatcher match, removing per-call timer + closure allocations
  from `Debounce`/`Throttle`.
- **`KeepAliveTimer.Nudge`** dropped a redundant lock — both `Nudge` and the
  timer tick run on the UI thread already, so the lock only serialised zoom
  interactions.
- **`LabelTextInfo.EnableCopyToClipboard`** caches the copy `ContextMenu` and
  swaps it in/out, instead of allocating a fresh menu + bindings on every flip.
- **`ClipBorder`** caches the combined "border ring" geometry across renders.
  The two `StreamGeometry`s were already cached in `ArrangeOverride`, but the
  `Geometry.Combine(...)` call in `OnRender` (3 code paths) was rebuilding the
  ring on every render. The new `borderRingGeometryCache` field is invalidated
  alongside the inputs and rebuilt lazily; brush-only changes (which trigger
  `OnRender` but not `ArrangeOverride`) now reuse the cached ring.

### Documentation

- **Project READMEs** added for the previously-bare projects: `Atc.Wpf`,
  `Atc.Wpf.Controls`, `Atc.Wpf.Components`, `Atc.Wpf.Theming`, `Atc.Wpf.FontIcons`.
  Each gives a short orientation, a "what lives here" map, an install snippet,
  and links into `docs/`.
- **Top-level doc indexes** added: `docs/Forms/@Readme.md` and
  `docs/Components/@Readme.md` — conceptual overviews that cover the deferred
  validation pattern, dispose / Loaded / Unloaded conventions, and pointers to
  per-control readmes.
- **Sample app README** added at `sample/Atc.Wpf.Sample/Readme.md` — quick-start,
  TreeView category map, search syntax, theme-switching tips, and a how-to for
  contributing a new sample.
- **`docs/SourceGenerators/ViewModel.md` TODOs** resolved — `ShowData` opens an
  `InfoDialogBox` with formatted person info; `CanSaveHandler` validates
  first/last/age non-empty.
- **`NumericBox_Readme.md`** added — full reference for the base numeric input
  class (value/range/interval, formatting/culture, input behaviour, spin-button
  layout, routed events).
- **`ThicknessBox_Readme.md`** added — short reference for the four-up
  `Thickness` editor.
- **`CONTRIBUTING.md`** added at the repo root — covers build / test / PR
  checklist, project structure, and links to the ATC umbrella guidelines.
- **`CODE_OF_CONDUCT.md`** added — adopts Contributor Covenant 2.1 by reference.
- **`SECURITY.md`** added — documents private vulnerability reporting via the
  GitHub Security tab and the supported-version policy.
- **`CLAUDE.md`** test counts and Microsoft Testing Platform invocation refreshed
  (`dotnet run --project ...` instead of `dotnet test <dir>`); the `Components`
  and `FontIcons` test rows were added.
- **DocFX docs site scaffold** added — `docfx.json` at the repo root pulls API
  metadata from all 8 source projects, drops the generated reference under
  `docs/api/`, and builds the existing `docs/` markdown plus a new `index.md`
  landing page with a curated section index. `toc.yml` (root) wires Home /
  Documentation / API Reference / GitHub; `docs/toc.yml` orders the left nav
  by category. `_site/` and `docs/api/` are gitignored. Local preview:
  `dotnet tool install -g docfx && docfx docfx.json --serve`.
- **Architecture diagram** added to `README.md` under the "🎯 Four-Tier Architecture"
  section. Mermaid graph (renders natively on GitHub, no PNG/SVG asset to maintain)
  showing `Atc.Wpf` (core) → `Controls` → `Forms` → `Components` spine plus
  `Theming` / `FontIcons` / `Network` / `UndoRedo` side packages and the upstream
  `Atc` + `Atc.XamlToolkit` deps. Color-coded by role.
- **MVVM migration guide** added at `docs/Mvvm/Migration.md`. Covers all four
  generator attributes (`[ObservableProperty]`, `[RelayCommand]`,
  `[DependencyProperty]`, `[AttachedProperty]`) with before/after snippets, a
  quick-reference cookbook, a pre-flight checklist, and notes on known
  limitations (e.g. the DP generator's missing XML-doc forwarding). Linked from
  `docs/Mvvm/@Readme.md` and added to `docs/toc.yml`.

### Removed

- **Empty `src/Atc.Wpf.SourceGenerators/` and `test/Atc.Wpf.SourceGenerators.Tests/`
  directories** — both contained zero files, were not referenced from
  `Atc.Wpf.slnx`, and would have implied a generator project that doesn't exist
  here (the actual generators ship via `Atc.XamlToolkit` / `Atc.XamlToolkit.Wpf`
  NuGet packages). `CLAUDE.md` caveat updated accordingly.

### Tests

- **`Atc.Wpf.Benchmarks`** project added (referenced from `Atc.Wpf.slnx` under
  a new `/benchmark/` solution folder) — BenchmarkDotNet 0.13.4 pilot. Lives
  outside `test/` so `dotnet test` never picks it up, but is part of the
  solution so a plain `dotnet build` keeps it from rotting. First benchmark
  (`SolidColorBrushHelperBenchmarks`, `[MemoryDiagnoser]`, parameterised on
  four colour names) pins the post-fix O(1) reverse lookup in
  `SolidColorBrushHelper.GetBrushFromString` so future regressions surface
  immediately. Verified locally: ~10–12 ns/op with **0 allocations** per call
  on a 12th-gen i9. `BenchmarkConfig.Create()` overrides the BDN toolchain to
  `net10.0-windows` (the auto-generated boilerplate project otherwise defaults
  to plain `net10.0` and fails to restore against our WPF-targeted assembly
  with NU1201). `benchmark/Directory.Build.props` relaxes the strict analyser
  set (CA1707/CA1304/CA1822/SA1623/CA1812/MA0051) because BenchmarkDotNet
  conventions clash with them. `benchmark/Atc.Wpf.Benchmarks/Readme.md`
  documents how to run (`dotnet run -c Release --project benchmark/Atc.Wpf.Benchmarks -- --filter '*'`),
  filtering, the WPF/toolchain gotcha (CLI `--job short` adds a *second* job
  with the default toolchain that breaks; use job attributes or
  `--iterationCount`/`--launchCount`/`--warmupCount` instead), and the
  convention to add a benchmark whenever a measurable hot-path fix lands.
- **`Atc.Wpf.UiTests`** project added (referenced from `Atc.Wpf.slnx`) — pilot
  visual / UI regression harness on **FlaUI 5.0.0** (`FlaUI.Core` + `FlaUI.UIA3`).
  `SampleAppPath.Resolve()` walks up to the repo root to find the sample exe.
  Two opt-in tests (`[Trait("Category","UI")]` + `[Fact(Skip="...")]` so they
  stay out of headless CI by default):
  - `SampleAppSmokeTests.Sample_app_launches_and_shows_main_window` — launches
    the sample, asserts a non-empty title, captures the main window to
    `bin/<Config>/net10.0-windows/Snapshots/sample-app-main-window.png`.
  - `NiceWindowSnapshotTests.NiceWindow_chrome_is_active_and_titlebar_strip_can_be_snapshotted` —
    asserts `mainWindow.ClassName == "NiceWindow"` (proves the custom themed
    chrome is in play, not a fallback `Window`), maximizes the window so it's
    deterministically on top, captures the full window plus a cropped 40-px
    title-bar strip — the highest-risk theming surface (accent colors, system
    buttons, title text), isolated for low-noise future image diffs.
  Teardown was hardened in both tests: `WaitWhileMainHandleIsMissing` races
  with `Close()` on fast machines and FlaUI throws a wrapped `System.Exception`,
  so cleanup is now wrapped in a broad try/catch. The Readme documents the
  Z-order trap (`Capture()` clips to element bounds but reads from the screen
  as backing store, so the window must be foregrounded; `SetForeground()`
  alone is blocked by Windows for non-foreground processes — maximize via the
  Window pattern is the workaround), the `Bitmap.Clone` overload trap, and the
  pattern for new tests.
- **`Atc.Wpf.FontIcons.Tests`** project added (referenced from `Atc.Wpf.slnx`).
  Adds an `IAssemblyMarkerAtcWpfFontIcons` to the source assembly and a small
  enum smoke-test suite that asserts every icon set defines `None = 0` and
  exposes more than just `None` — catches generator regressions across all
  ten icon sets (FontAwesome 5/7 solid/regular/brand, Bootstrap, Material,
  Weather, IcoFont). 21 tests pass.
- **Removed dead test-fixture duplicates from `Atc.Wpf.Controls.Tests`.** Five
  files under `XUnitTestTypes/` (`Account` / `Address` / `DriveItem` / `Person` /
  `PrimitiveTypesModel`) were byte-identical copies of the same files in
  `Atc.Wpf.Forms.Tests` but had no consumers in `Controls.Tests`. Deleted them
  and the now-empty folder; also dropped two `System.ComponentModel*` global
  usings that became unused.
- **`Atc.Wpf.Theming.Tests` upgraded from compliance-only to functional**
  (1 → 43 tests). New value-converter test files: `CornerRadiusBindingValueConverterTests`,
  `CornerRadiusFilterValueConverterTests`, `LeftRightCornerRadiusValueConverterTests`,
  `ColorToNameValueConverterTests`, `RenderColorIndicatorTypeToVisibilityValueConverterTests`.
  Together they cover the per-corner / per-side / single-corner switch logic,
  `IgnoreRadius` / `Filter` property fallbacks, invalid-input fallbacks
  (`Binding.DoNothing`, `default(CornerRadius)`, `Colors.Pink`),
  `ConvertBack` semantics (`DependencyProperty.UnsetValue` for one-ways,
  `NotSupportedException` for multi-binding back-conversion), and
  `Visibility` mapping for `RenderColorIndicatorType`.
- **`Atc.Wpf.Components.Tests` extended with pure-logic coverage** (164 → 177
  tests). New test files: `FlyoutFormResultFactoryTests` (Success / Cancelled /
  ValidationFailed semantics on the non-generic factory), `DualListSelectorItemTests`
  (POCO defaults + `ToString`), `DualListSelectorItemsReorderedEventArgsTests`,
  `DualListSelectorItemsTransferredEventArgsTests` (both directions + empty-items
  case), `TerminalReceivedDataEventArgsTests` (line-array exposure + `ToString`).
- **`Atc.Wpf.Tests` extended with core value-converter coverage** (818 → 869
  tests, +51). New test files: `ThicknessBindingValueConverterTests` (per-side
  zero-out + `IgnoreThicknessSide` property fallback + invalid-input default),
  `ThicknessFilterValueConverterTests` (per-side keep-only + `Filter` property
  fallback + `Binding.DoNothing`), `MathValueConverterTests` (single-binding
  `value`/`parameter` operands across +/-/×/÷, multi-binding first-two-values
  operands, divide-by-non-positive guard, null-operand fallback, non-numeric
  fallback, `ConvertBack` semantics for both the single and multi shapes),
  `RectangleCircularValueConverterTests` (half-of-min-dimension + zero-dimension
  + wrong-length + non-double + `NotSupportedException` on `ConvertBack`),
  `ColorHexToColorValueConverterTests` (Color → `AARRGGBB`, hex → Color
  including without `#` prefix and 7-char form, null + invalid-length →
  `Binding.DoNothing`, round-trip). Follow-up batch (+38 tests, 869 → 907):
  `ColorToSolidColorValueConverterTests` (alpha-to-255 normalisation + DeepPink
  null fallback + wrong-type guard + `NotSupportedException` on `ConvertBack`),
  `BackgroundToForegroundValueConverterTests` (ideal text colour against dark
  vs light background, frozen brush invariant, multi-binding explicit-title
  passthrough + multi-binding `ConvertBack` returns one `UnsetValue` per
  `targetType`), `WindowResizeModeMinMaxButtonVisibilityMultiValueConverterTests`
  (full Min/Max/Close × NoResize/CanMinimize/CanResize/CanResizeWithGrip
  matrix, `useNoneWindowStyle` and `showButton` precedence, null-values and
  wrong-parameter safe fallbacks), `MethodToValueConverterTests` (parameterless
  method invocation via reflection cache, instance-state preservation across
  calls, missing-method / wrong-parameter / null-value safe fallbacks,
  `NotSupportedException` on `ConvertBack`),
  `ObservableDictionaryToDictionaryOfStringsValueConverterTests` (string-key,
  int-key supported variants + null → empty + unsupported-type throws +
  `NotSupportedException` on `ConvertBack`). JSON-tree batch (+20 tests, 907
  → 927): `JsonArrayLengthConverterTests` (`[N]` for array nodes vs `[ N ]`
  padded form for properties holding an array, empty string for non-array
  property + unsupported input), `JsonNodeChildrenConverterTests` (object
  property children, array item children, empty for value nodes, null for
  unsupported input), `JsonValueDisplayConverterTests` (returns `DisplayValue`
  for string + numeric value nodes, passes non-`JsonValueNode` input through
  unchanged). With this batch, every previously-untested **public** value
  converter in `Atc.Wpf` now has functional tests. Extensions / Collections
  batch (+44 tests, 927 → 971): `CornerRadiusExtensionsTests` (`IsValid` matrix
  across negative / NaN / ±∞ flags, `IsZero`, `IsUniform`),
  `ThicknessExtensionsTests` (same shape + `CollapseThickness` width/height
  sum), `RectExtensionsTests` (`Deflate` shrink + clamp-to-zero when thickness
  exceeds size, `Inflate` expansion), `ColorExtensionsTests` (`Lerp` at 0 / 0.5
  / 1, HSB `GetHue` for pure red/green/blue, `GetBrightness` for black / white),
  `ObservableDictionaryTests` (Add / `ContainsKey` / Remove / `TryGetValue` /
  indexer / `Keys` / `Values` / duplicate-key throw, plus the `CopyTo`
  implementation added earlier this session — null array, negative `arrayIndex`,
  insufficient destination size guards). Follow-up batch (+18 tests, 971 →
  989): `GradientStopCollectionExtensionsTests` (`GetColorAtOffset` clamps
  below 0 and above 1, returns first/last stop at the boundaries, interpolates
  to the truncated mid-grey at 0.5 — note that `(255 * 0.5) + 0 = 127.5` casts
  to byte 127, not 128 — interpolates within a single segment of a three-stop
  gradient, and sorts unsorted stops internally), `ObservableKeyValuePairTests`
  (`INotifyPropertyChanged` for both `Key` and `Value`, multiple-event capture,
  direct `OnPropertyChanged` invocation), `ObservableDictionaryExtensionsTests`
  (all three `ToDictionaryOfStrings` overloads — string keys passed through,
  int keys via invariant `ToString`, Guid keys via default Guid format —
  plus null-input guards on each). Follow-up batch (+23 tests, 989 → 1012):
  `ObservableCollectionExTests` (`AddRange` raises a single `Add` event for the
  whole batch, `Refresh` raises a single `Reset`, toggling
  `SuppressOnChangedNotification` to the same value is a no-op, toggling
  off after suppressed mutations raises a `Reset`, individual `Add` outside
  `AddRange` still raises one event per call, null-input throws),
  `BrushExtensionsTests` (`IsOpaqueSolidColorBrush` true/false matrix on
  `SolidColorBrush` alpha + non-solid types, `IsEqualTo` symmetric / different-
  type / matching solid / different-color / different-opacity / matching linear-
  gradient / different-stop-count linear / different-radius radial / null-
  guard), `DrawingGroupExtensionsTests` (`ApplyTransform` assigns directly on
  no-existing-transform, appends to existing `TransformGroup`, wraps existing
  non-group transform into a new `TransformGroup`, throws on null drawing).
- **`Atc.Wpf.Forms.Tests` extended with pure-logic coverage** (148 → 166
  tests, +18). New test files: `InMemoryFontPickerStorageTests` (LRU
  promote-on-rerecord, blank-input ignore, `MaxRecentItems` cap, snapshot
  isolation between reads and later writes), `FontPickerStorageTests` (default
  `Current` is `InMemoryFontPickerStorage`, null-guard on setter, custom
  storage replaces the singleton), `LabelControlDataTests` (POCO defaults +
  round-trip + `ToString`), `LabelInputFormPanelSettingsTests` (defaults +
  round-trip + `ToString`).
- **`Atc.Wpf.Controls.Tests` extended with value-converter and event-arg
  coverage** (387 → 438 tests, +51). New test files:
  `IntegerToDoubleValueConverterTests` (pinning the per-arm boxing semantics —
  `int i => i` keeps `int` because there's no common numeric type across
  `int` / `decimal` / `double` arms; `ConvertBack`'s `_ => 0` is implicitly
  promoted to `0d` because the sibling arm is `double`),
  `NetworkProtocolToStringValueConverterTests` (all 8 protocols + 7 schemes
  round-trip, whitespace trimming, blank/null fallbacks, `Binding.DoNothing`
  for unsupported input), `RenderFlagIndicatorTypeToVisibilityValueConverterTests`
  (Visible/Collapsed mapping + `ArgumentNullException` + wrong-type/wrong-parameter
  guards + `NotSupportedException` on `ConvertBack`),
  `StepperStepChangedEventArgsTests`, `StepperStepChangingEventArgsTests`
  (`Cancel` defaults to false + setter), `SegmentedSelectionChangedEventArgsTests`
  (null items allowed + index pair).
